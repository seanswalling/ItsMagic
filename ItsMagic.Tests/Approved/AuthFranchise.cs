using Mercury.Core;
using Mercury.Core.NHibernateExtensions;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;


namespace Mercury.Plugins.AuthorisationReadModel
{
    public class AuthFranchise
    {
        public virtual Guid Id { get; set; }
        public virtual Guid? FranchiseGroupId { get; set; }
        public virtual string Name { get; set; }
    }

    public class AuthFranchiseMap : ClassMapping<AuthFranchise>
    {
        public AuthFranchiseMap()
        {
            Id(x => x.Id, m =>
            {
                m.Column("Id");
                m.Generator(Generators.Assigned);
            });
            Property(x => x.FranchiseGroupId, m => m.Nullable());
            Property(x => x.Name, m =>
            {
                m.NotNullable();
                m.Length(200);
            });
            Schema(Constants.AuthorisationSchema);
        }
    }
}