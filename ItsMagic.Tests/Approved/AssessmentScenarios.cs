using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mercury.Common.ReferenceData;
using Mercury.Contracts.Assessments;
using Mercury.Contracts.Assessments.Events;
using Mercury.Contracts.BankStatements.Events;
using Mercury.Contracts.Events.InStoreLoanApplications;
using Mercury.Contracts.Identification;
using Mercury.Contracts.OnlineLoanApplications.Events;
using Mercury.Contracts.ReferenceData;
using Mercury.Contracts.Tests.ObjectMothers;
using Mercury.Contracts.Tests.ObjectMothers.Assessments;
using Mercury.Core.JsonExtensions;
using Mercury.Tests.Core;
using Mercury.Tests.Core.Scenarios;
using Ploeh.AutoFixture;

namespace Mercury.Contracts.Tests.Scenarios
{
    public static class AssessmentScenarios
    {
        public static IEnumerable<Scenario> APersonalLoanAssessmentIsCreated(Guid? assessmentId = null)
        {
            yield return Scenario.ChainedScenario(sc =>
            {
                var lastApplicationData = GetLastPersonalLoanApplication(sc);

                if (lastApplicationData == null)
                {
                    return AssessmentObjectMother.Events.PersonalLoanAssessmentCreatedEvent.FromAny.Create();
                }

                PersonalLoanAssessmentCreatedEvent personalLoanAssessmentCreatedEvent;

                if (lastApplicationData.InStoreLoanApplicationId.HasValue)
                {
                    personalLoanAssessmentCreatedEvent = AssessmentObjectMother.Events.PersonalLoanAssessmentCreatedEvent
                        .FromInStoreApplication
                        .With(x => x.InStoreLoanApplicationId, lastApplicationData.InStoreLoanApplicationId)
                        .Create();
                }
                else
                {
                    personalLoanAssessmentCreatedEvent = AssessmentObjectMother.Events.PersonalLoanAssessmentCreatedEvent
                        .FromOnlineApplication
                        .With(x => x.OnlineLoanApplicationId, lastApplicationData.OnlineLoanApplicationId)
                        .Create();
                }

                personalLoanAssessmentCreatedEvent.OriginatingStoreId = lastApplicationData.OriginatingStoreId;
                personalLoanAssessmentCreatedEvent.CustomerId = lastApplicationData.CustomerId ?? sc.TryFindCustomerId() ?? Guid.NewGuid();
                personalLoanAssessmentCreatedEvent.IsNewCustomer = !lastApplicationData.CustomerId.HasValue;
                personalLoanAssessmentCreatedEvent.IdentificationDetails = lastApplicationData.IdentificationDetails;

                if (lastApplicationData.ProvidedDocuments != null)
                {
                    personalLoanAssessmentCreatedEvent.ProvidedDocuments = lastApplicationData.ProvidedDocuments;
                }

                return personalLoanAssessmentCreatedEvent.AnnotateEventMetadata(sc);
            });
        }

        public static IEnumerable<Scenario> MissingReviewerDocumentsUpdatedWithCcStatements(int numBankStatements, Guid? assessmentId = null)
        {
            yield return Scenario.ChainedScenario(sc =>
            {
                var missingReviewerDocumentsUpdatedEvent = new MissingReviewerDocumentsUpdatedEvent
                {
                    MissingDocumentsTaskId = Guid.NewGuid()

                }.AnnotateAssessmentMetadata(sc, assessmentId);

                var bankStatementRetrievedEvents = sc.Events.OfType<BankStatementRetrievedEvent>();

                if (missingReviewerDocumentsUpdatedEvent.OnlineLoanApplicationId.HasValue)
                {
                    bankStatementRetrievedEvents = bankStatementRetrievedEvents
                        .Where(e => e.OnlineApplicationId.Value == missingReviewerDocumentsUpdatedEvent.OnlineLoanApplicationId.Value)
                        .Reverse()
                        .Take(numBankStatements)
                        .Reverse()
                        .ToArray();
                }
                else
                {
                    bankStatementRetrievedEvents = bankStatementRetrievedEvents
                        .Where(e => e.StoreId == missingReviewerDocumentsUpdatedEvent.OriginatingStoreId)
                        .Reverse()
                        .Take(numBankStatements)
                        .Reverse()
                        .ToArray();
                }

                if (bankStatementRetrievedEvents.Count() < numBankStatements)
                {
                    throw new ArgumentException(string.Format("{0} bank statement retrieved event(s) not found", numBankStatements));
                }

                var previousMissingReviewerDocumentsUpdatedEvent = sc.Events
                    .OfType<MissingReviewerDocumentsUpdatedEvent>()
                    .LastOrDefault(e => e.AggregateId == missingReviewerDocumentsUpdatedEvent.AggregateId);

                var bankDocuments = bankStatementRetrievedEvents.Select(e =>
                    new DocumentSummary
                    {
                        DocumentType = DocumentType.BankStatement,
                        DocumentId = e.DocumentId,
                        Source = DocumentSource.CCStatement,
                        Description = e.StatementRef,
                        NumberOfPages = 1,
                        UploadedAt = sc.Now
                    });

                if (previousMissingReviewerDocumentsUpdatedEvent != null)
                {
                    missingReviewerDocumentsUpdatedEvent.MissingReviewerDocuments = previousMissingReviewerDocumentsUpdatedEvent.MissingReviewerDocuments.JsonCopy()
                        .Concat(bankDocuments)
                        .ToArray();
                }
                else
                {
                    missingReviewerDocumentsUpdatedEvent.MissingReviewerDocuments = bankDocuments.ToArray();
                }

                return missingReviewerDocumentsUpdatedEvent;
            });
        }

        public static IEnumerable<Scenario> MissingDocumentsAreUploaded(Guid? assessmentId = null)
        {
            yield return new ChainedScenario(sc =>
            {
                var missingReviewerDocumentsUpdatedEvent = sc.Events
                    .OfType<MissingReviewerDocumentsUpdatedEvent>()
                    .LastOrDefault(e => e.AggregateId == (assessmentId ?? e.AggregateId));
                return AssessmentObjectMother.Events.MissingDocumentsUploadedEvent.New
                    .With(
                        x => x.Documents,
                        missingReviewerDocumentsUpdatedEvent.MissingReviewerDocuments.JsonCopy())
                    .Create()
                    .AnnotateAssessmentMetadata(sc, assessmentId);
            });
        }

        public static AssessmentManuallyApprovedScenarioBuilder AnAssessmentIsManuallyApproved(Guid? assessmentId = null)
        {
            return new AssessmentManuallyApprovedScenarioBuilder(assessmentId);
        }

        public static IEnumerable<Scenario> AnAssessmentIsDeclinedPartIX(Guid? assessmentId = null)
        {
            yield return new ChainedScenario(sc =>
            {
                var assessmentDeclinedPartIXEvent = AssessmentObjectMother.Events.AssessmentDeclinedPartIXEvent
                    .FromAnyApplication
                    .Create()
                    .AnnotateAssessmentMetadata(sc, assessmentId);

                var personalLoanAssessmentCreatedEvent = sc.Events.OfType<PersonalLoanAssessmentCreatedEvent>().SingleOrDefault(e => e.AggregateId == assessmentDeclinedPartIXEvent.AggregateId);

                assessmentDeclinedPartIXEvent.IdentificationDetails = personalLoanAssessmentCreatedEvent.IdentificationDetails;

                return assessmentDeclinedPartIXEvent;
            });
        }

        public static IEnumerable<Scenario> AnAssessmentIsManuallyDeclined(Guid? assessmentId = null)
        {
            yield return new ChainedScenario(sc =>
            {
                var assessmentManuallyDeclinedEvent = AssessmentObjectMother.Events.AssessmentManuallyDeclinedEvent
                    .FromAnyApplication
                    .Create()
                    .AnnotateAssessmentMetadata(sc, assessmentId);

                var personalLoanAssessmentCreatedEvent = sc.Events.OfType<PersonalLoanAssessmentCreatedEvent>().SingleOrDefault(e => e.AggregateId == assessmentManuallyDeclinedEvent.AggregateId);

                assessmentManuallyDeclinedEvent.IdentificationDetails = personalLoanAssessmentCreatedEvent.IdentificationDetails;

                return assessmentManuallyDeclinedEvent;
            });
        }

        public static IEnumerable<Scenario> AnAssessmentIsManuallyDeclinedFromMissingDocuments(Guid? assessmentId = null)
        {
            yield return new ChainedScenario(sc =>
            {
                var assessmentManuallyDeclinedFromMissingDocumentsEvent = AssessmentObjectMother.Events.AssessmentManuallyDeclinedFromMissingDocumentsEvent
                    .FromAnyApplication
                    .Create()
                    .AnnotateAssessmentMetadata(sc, assessmentId);

                var personalLoanAssessmentCreatedEvent = sc.Events.OfType<PersonalLoanAssessmentCreatedEvent>().SingleOrDefault(e => e.AggregateId == assessmentManuallyDeclinedFromMissingDocumentsEvent.AggregateId);

                assessmentManuallyDeclinedFromMissingDocumentsEvent.IdentificationDetails = personalLoanAssessmentCreatedEvent.IdentificationDetails;

                return assessmentManuallyDeclinedFromMissingDocumentsEvent;
            });
        }

        public static IEnumerable<Scenario> AnAssessmentIsWithdrawn(Guid? assessmentId = null)
        {
            yield return new ChainedScenario(sc =>
            {
                var assessmentWithdrawnEvent = AssessmentObjectMother.Events.AssessmentWithdrawnEvent
                    .FromAnyApplication
                    .Create()
                    .AnnotateAssessmentMetadata(sc, assessmentId);

                var personalLoanAssessmentCreatedEvent = sc.Events.OfType<PersonalLoanAssessmentCreatedEvent>().SingleOrDefault(e => e.AggregateId == assessmentWithdrawnEvent.AggregateId);

                assessmentWithdrawnEvent.IdentificationDetails = personalLoanAssessmentCreatedEvent.IdentificationDetails;

                return assessmentWithdrawnEvent;
            });
        }

        public static IEnumerable<Scenario> AnAssessmentNoteIsUpdatedWithHistoryScores(Guid? assessmentId = null)
        {
            yield return new ChainedScenarios(sc =>
            {
                return ABankingHistoryScoreIsModified(assessmentId)
                    .Concat(ALoanHistoryScoreIsModified(assessmentId))
                    .Concat(ACreditHistoryScoreIsModified(assessmentId))
                    .Concat(AnAssessmentNoteIsUpdated(assessmentId));
            });
        }

        public static IEnumerable<Scenario> ABankingHistoryScoreIsModified(Guid? assessmentId = null)
        {
            yield return new ChainedScenario(sc =>
            {
                return AssessmentEventsObjectMother.BuildBankingHistoryScoreModifiedEvent()
                    .AnnotateAssessmentMetadata(sc, assessmentId);
            });
        }

        public static IEnumerable<Scenario> ALoanHistoryScoreIsModified(Guid? assessmentId = null)
        {
            yield return new ChainedScenario(sc =>
            {
                return AssessmentEventsObjectMother.BuildLoanHistoryScoreModifiedEvent()
                    .AnnotateAssessmentMetadata(sc, assessmentId);
            });
        }

        public static IEnumerable<Scenario> ACreditHistoryScoreIsModified(Guid? assessmentId = null)
        {
            yield return new ChainedScenario(sc =>
            {
                return AssessmentEventsObjectMother.BuildCreditHistoryScoreModifiedEvent()
                    .AnnotateAssessmentMetadata(sc, assessmentId);
            });
        }

        public static IEnumerable<Scenario> AnAssessmentNoteIsUpdated(Guid? assessmentId = null)
        {
            yield return new ChainedScenario(sc =>
            {
                return AssessmentObjectMother.Events.NotesUpdatedEvent
                    .New
                    .Create()
                    .AnnotateAssessmentMetadata(sc, assessmentId);
            });
        }

        public static AssessmentAssociatedWithExistingCustomerScenarioBuilder AnAssessmentIsAssociatedWithCustomer(Guid? assessmentId = null)
        {
            return new AssessmentAssociatedWithExistingCustomerScenarioBuilder(assessmentId);
        }

        public static FinancialDetailsUpdatedScenarioBuilder FinancialDetailsAreUpdated(Guid? assessmentId = null)
        {
            return new FinancialDetailsUpdatedScenarioBuilder(assessmentId);
        }

        public static FinancialDetailsVerifiedScenarioBuilder FinancialDetailsAreVerified()
        {
            return new FinancialDetailsVerifiedScenarioBuilder();
        }

        public static IEnumerable<Scenario> AnAssessmentIsClosed(ManualAssessmentScreen screen, Guid? assessmentId = null)
        {
            yield return Scenario.ChainedScenario(sc =>
            {
                return AssessmentEventsObjectMother.BuildAssessmentClosedEvent(screen)
                    .AnnotateAssessmentMetadata(sc, assessmentId);
            });
        }

        private static TEvent AnnotateAssessmentMetadata<TEvent>(
            this TEvent e,
            ScenarioCollection sc,
            Guid? assessmentId = null)
            where TEvent : AssessmentEventBase
        {
            var personalLoanAssessmentCreatedEvent = GetLastPersonalLoanAssessmentCreatedEvent(sc, assessmentId);

            e.AggregateId = personalLoanAssessmentCreatedEvent.AggregateId;
            e.CustomerId = personalLoanAssessmentCreatedEvent.CustomerId;
            e.InStoreLoanApplicationId = personalLoanAssessmentCreatedEvent.InStoreLoanApplicationId;
            e.OnlineLoanApplicationId = personalLoanAssessmentCreatedEvent.OnlineLoanApplicationId;
            e.OriginatingStoreId = personalLoanAssessmentCreatedEvent.OriginatingStoreId;
            e.CommandTimestamp = e.CommandTimestamp ?? sc.Now;

            return e.AnnotateCustomerIdByAssociationEvent(sc).AnnotateEventMetadata(sc);
        }

        private static PersonalLoanAssessmentCreatedEvent GetLastPersonalLoanAssessmentCreatedEvent(
            ScenarioCollection sc,
            Guid? assessmentId)
        {
            var personalLoanAssessmentCreatedEvent = sc.Events
                .OfType<PersonalLoanAssessmentCreatedEvent>()
                .LastOrDefault(e => e.AggregateId == (assessmentId ?? e.AggregateId));

            if (personalLoanAssessmentCreatedEvent == null)
            {
                throw new ArgumentException("Could not find personal loan assessment created event to annotate assessment metadata");
            }

            return personalLoanAssessmentCreatedEvent;
        }

        private static TEvent AnnotateCustomerIdByAssociationEvent<TEvent>(
            this TEvent assessmentEventBase,
            ScenarioCollection sc)
            where TEvent : AssessmentEventBase
        {
            var associatedEvent = sc.Events
                .OfType<AssessmentAssociatedWithExistingCustomerEvent>()
                .LastOrDefault(e => e.AggregateId == assessmentEventBase.AggregateId);

            if (associatedEvent != null)
            {
                assessmentEventBase.CustomerId = associatedEvent.CustomerId;
            }

            return assessmentEventBase;
        }
        
        public class ApplicationData
        {
            public Guid? InStoreLoanApplicationId { get; set; }
            public Guid? OnlineLoanApplicationId { get; set; }
            public Guid OriginatingStoreId { get; set; }
            public Guid? CustomerId { get; set; }
            public DocumentSummary[] ProvidedDocuments { get; set; }
            public IdentificationDetailsSet IdentificationDetails { get; set; }
        }

        public static ApplicationData GetLastPersonalLoanApplication(ScenarioCollection sc)
        {
            return sc.Events
                .OfType<ApplicationSubmittedEvent>()
                .Where(e => e.PersonalLoanConfiguration != null)
                .Select(e => new ApplicationData
                {
                    InStoreLoanApplicationId = e.AggregateId,
                    OriginatingStoreId = e.StoreId,
                    CustomerId = e.ExistingCustomerId,
                    ProvidedDocuments = e.AttachedDocumentation,
                    IdentificationDetails = e.IdentificationDetails
                })
                .Concat(
                    sc.Events
                        .OfType<InStorePersonalLoanApplicationSubmissionImportedEvent>()
                        .Select(e => new ApplicationData
                        {
                            InStoreLoanApplicationId = e.AggregateId,
                            OriginatingStoreId = e.StoreId,
                            CustomerId = e.ExistingCustomerId,
                            ProvidedDocuments = e.ProvidedDocuments,
                            IdentificationDetails = e.IdentificationDetails
                        }))
                .Concat(
                    sc.Events
                        .OfType<OnlineApplicationSubmittedEvent>()
                        .Select(e => new ApplicationData
                        {
                            OnlineLoanApplicationId = e.AggregateId,
                            OriginatingStoreId = Backoffice.StoreId,
                            CustomerId = null,
                            ProvidedDocuments = null,
                            IdentificationDetails = e.IdentificationDetails
                        }))
                .Concat(
                    sc.Events
                        .OfType<OnlineApplicationReviews.Events.OnlineApplicationReviewCompletedEvent>()
                        .Where(e => e.LoanType != LoanType.CashAdvance)
                        .Select(e => new ApplicationData
                        {
                            OnlineLoanApplicationId = e.OnlineApplicationId,
                            OriginatingStoreId = e.StoreId,
                            CustomerId = e.CustomerId,
                            ProvidedDocuments = e.ProvidedDocuments,
                            IdentificationDetails = e.IdentificationDetails
                        }))
                .Concat(
                    sc.Events
                        .OfType<OnlinePersonalLoanApplicationSubmissionImportedEvent>()
                        .Select(e => new ApplicationData
                        {
                            OnlineLoanApplicationId = e.AggregateId,
                            OriginatingStoreId = e.OriginatingStoreId,
                            CustomerId = e.CustomerId,
                            ProvidedDocuments = e.ProvidedDocuments,
                            IdentificationDetails = e.IdentificationDetails
                        }))
                .LastOrDefault();
        }

        public class AssessmentManuallyApprovedScenarioBuilder : IEnumerable<Scenario>
        {
            private readonly Guid? _assessmentId;
            private LoanConfiguration _approvedLoanConfiguration;

            public AssessmentManuallyApprovedScenarioBuilder(Guid? assessmentId)
            {
                _assessmentId = assessmentId;
            }

            public AssessmentManuallyApprovedScenarioBuilder WithPersonalLoanConfiguration()
            {
                _approvedLoanConfiguration = Randomness.Instance.PersonalLoanConfiguration();
                return this;
            }

            public AssessmentManuallyApprovedScenarioBuilder WithMaccConfiguration()
            {
                _approvedLoanConfiguration = Randomness.Instance.MaccConfiguration();
                return this;
            }

            public AssessmentManuallyApprovedScenarioBuilder WithChangedLoanConfiguration()
            {
                return Randomness.Instance.Bool()
                    ? WithPersonalLoanConfiguration()
                    : WithMaccConfiguration();
            }

            public IEnumerator<Scenario> GetEnumerator()
            {
                yield return new ChainedScenario(sc =>
                {
                    var createdEvent = sc.Events
                        .OfType<PersonalLoanAssessmentCreatedEvent>()
                        .Last(e => e.AggregateId == (_assessmentId ?? e.AggregateId));

                    var financialDetailsVerifiedEvent = sc.Events.OfType<FinancialDetailsVerifiedEvent>()
                        .LastOrDefault(e => e.AggregateId == createdEvent.AggregateId);
                    var verifiedIncomeAndExpenditure = financialDetailsVerifiedEvent == null
                        ? ContractsObjectMother.VerifiedIncomeAndExpenditure.New.Create()
                        : new VerifiedIncomeAndExpenditure
                        {
                            IncomeSources = financialDetailsVerifiedEvent.Income,
                            ExpenditureSources = financialDetailsVerifiedEvent.Expenditure,
                            ActiveLoanExpenditures = financialDetailsVerifiedEvent.ActiveLoanExpenditures,
                            RecentlyCompletedLoanExpenditures = financialDetailsVerifiedEvent.CompletedInLast90DaysLoanExpenditures,
                            VerifiedTimestamp = financialDetailsVerifiedEvent.Timestamp,
                            IncomeAndExpenditureSources = financialDetailsVerifiedEvent.IncomeAndExpenditure,
                            PresumptionOfHardship = financialDetailsVerifiedEvent.PresumptionOfHardship
                        };

                    return new AssessmentManuallyApprovedEvent
                    {
                        ApplicantAddressDetails = createdEvent.ApplicantAddressDetails,
                        ApplicantAlternativeContacts = createdEvent.ApplicantAlternativeContacts,
                        ApplicantPersonalDetails = createdEvent.ApplicantPersonalDetails,
                        ApprovedLoanConfiguration = _approvedLoanConfiguration ?? createdEvent.LoanConfiguration,
                        BankAccount = createdEvent.BankAccount,
                        InStoreLoanApplicationId = createdEvent.InStoreLoanApplicationId,
                        IdentificationDetails = createdEvent.IdentificationDetails,
                        IsInPresumptionOfHardship = (verifiedIncomeAndExpenditure != null)
                            && (verifiedIncomeAndExpenditure.PresumptionOfHardship != null)
                            && (verifiedIncomeAndExpenditure.PresumptionOfHardship.EvaluationResult != null)
                            && (verifiedIncomeAndExpenditure.PresumptionOfHardship.EvaluationResult.Triggers.Any()),
                        OnlineLoanApplicationId = createdEvent.OnlineLoanApplicationId,
                        VerifiedIncomeAndExpenditure = verifiedIncomeAndExpenditure
                    }.AnnotateAssessmentMetadata(sc);
                });
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class AssessmentAssociatedWithExistingCustomerScenarioBuilder : IEnumerable<Scenario>
        {
            private readonly Guid? _assessmentId;
            private Guid? _customerId;
            private bool _getLastCustomer;
            private bool _customerSet;
            private bool _customerFromOriginalApplication;

            public AssessmentAssociatedWithExistingCustomerScenarioBuilder(Guid? assessmentId)
            {
                _assessmentId = assessmentId;
            }

            public AssessmentAssociatedWithExistingCustomerScenarioBuilder ThatIsNew()
            {
                _customerId = null;
                _getLastCustomer = false;
                _customerFromOriginalApplication = false;
                _customerSet = true;
                return this;
            }

            public AssessmentAssociatedWithExistingCustomerScenarioBuilder ThatIsExisting(Guid? customerId = null)
            {
                _customerId = customerId;
                _getLastCustomer = !customerId.HasValue;
                _customerFromOriginalApplication = false;
                _customerSet = true;
                return this;
            }

            public AssessmentAssociatedWithExistingCustomerScenarioBuilder ThatIsFromOriginalLoanApplication()
            {
                _customerFromOriginalApplication = true;
                _getLastCustomer = false;
                _customerSet = true;
                return this;
            }

            public IEnumerator<Scenario> GetEnumerator()
            {
                yield return Scenario.ChainedScenario(sc =>
                {
                    if (!_customerSet)
                    {
                        throw new NotSupportedException("Customer not set");
                    }

                    var personalLoanAssessmentCreatedEvent = GetLastPersonalLoanAssessmentCreatedEvent(sc, _assessmentId);

                    var customerId = _customerFromOriginalApplication ? personalLoanAssessmentCreatedEvent.CustomerId : (_getLastCustomer ? sc.TryFindCustomerId() ?? Guid.NewGuid() : _customerId);

                    return AssessmentObjectMother.Events.AssessmentAssociatedWithExistingCustomerEvent
                        .FromAnyApplication
                        .With(x => x.AggregateId, personalLoanAssessmentCreatedEvent.AggregateId)
                        .With(x => x.InStoreLoanApplicationId, personalLoanAssessmentCreatedEvent.InStoreLoanApplicationId)
                        .With(x => x.OnlineLoanApplicationId, personalLoanAssessmentCreatedEvent.OnlineLoanApplicationId)
                        .With(x => x.CustomerId, customerId)
                        .Create()
                        .AnnotateEventMetadata(sc);
                });
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class FinancialDetailsUpdatedScenarioBuilder : IEnumerable<Scenario>
        {
            private readonly Guid? _assessmentId;
            private bool _isInPresumptionOfHardship = Randomness.Instance.Bool();

            public FinancialDetailsUpdatedScenarioBuilder(Guid? assessmentId)
            {
                _assessmentId = assessmentId;
            }

            public FinancialDetailsUpdatedScenarioBuilder WithPresumptionOfHardship()
            {
                _isInPresumptionOfHardship = true;
                return this;
            }

            public IEnumerator<Scenario> GetEnumerator()
            {
                yield return Scenario.ChainedScenario(sc =>
                {
                    var e = AssessmentEventsObjectMother.BuildFinancialDetailsUpdatedEvent(_assessmentId)
                        .AnnotateAssessmentMetadata(sc, _assessmentId);
                    e.IsInPresumptionOfHardship = _isInPresumptionOfHardship;

                    return e;
                });
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class FinancialDetailsVerifiedScenarioBuilder : IEnumerable<Scenario>
        {
            private bool _matchInstoreApplication;

            public FinancialDetailsVerifiedScenarioBuilder MatchingInstoreApplication()
            {
                _matchInstoreApplication = true;
                return this;
            }

            public IEnumerator<Scenario> GetEnumerator()
            {
                yield return Scenario.ChainedScenario(sc =>
                {
                    if(_matchInstoreApplication)
                    {
                        var instoreApplication = sc.Events.OfType<ApplicationSubmittedEvent>().Last();
                        return new FinancialDetailsVerifiedEvent
                        {
                            Income = instoreApplication.VerifiedIncomeAndExpenditure.IncomeSources,
                            Expenditure = instoreApplication.VerifiedIncomeAndExpenditure.ExpenditureSources,
                            ActiveLoanExpenditures = instoreApplication.VerifiedIncomeAndExpenditure.ActiveLoanExpenditures,
                            CompletedInLast90DaysLoanExpenditures = instoreApplication.VerifiedIncomeAndExpenditure.RecentlyCompletedLoanExpenditures,
                            IncomeAndExpenditure = instoreApplication.VerifiedIncomeAndExpenditure.IncomeAndExpenditureSources,
                            PresumptionOfHardship = instoreApplication.VerifiedIncomeAndExpenditure.PresumptionOfHardship
                        }.AnnotateAssessmentMetadata(sc);
                    }

                    var assessmentCreationEvent = sc.Events.OfType<PersonalLoanAssessmentCreatedEvent>().Last();
                    var financialDetailsUpdatedEvent = sc.Events.OfType<FinancialDetailsUpdatedEvent>().LastOrDefault(ev => ev.AggregateId == assessmentCreationEvent.AggregateId);
                    var artificial = ContractsObjectMother.VerifiedIncomeAndExpenditure.New.Create();
                    
                    return new FinancialDetailsVerifiedEvent
                    {
                        Income = financialDetailsUpdatedEvent == null ? artificial.IncomeSources : financialDetailsUpdatedEvent.UpdatedIncome,
                        Expenditure = financialDetailsUpdatedEvent == null ? artificial.ExpenditureSources : financialDetailsUpdatedEvent.UpdatedExpenditure,
                        ActiveLoanExpenditures = artificial.ActiveLoanExpenditures,
                        CompletedInLast90DaysLoanExpenditures = artificial.RecentlyCompletedLoanExpenditures,
                        IncomeAndExpenditure = financialDetailsUpdatedEvent == null ? artificial.IncomeAndExpenditureSources : financialDetailsUpdatedEvent.UpdatedIncomeAndExpenditure,
                        PresumptionOfHardship = PresumptionOfHardshipObjectMother.CreateDetails(financialDetailsUpdatedEvent != null && financialDetailsUpdatedEvent.IsInPresumptionOfHardship)
                    }.AnnotateAssessmentMetadata(sc);
                });
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
