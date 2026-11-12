using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using GreatCompany.Data;
using GreatCompany.Data.Mappings;
using NHibernate.Cfg;
using QS.Deletion.Configuration;
using QS.Project.HibernateMapping;

namespace GreatCompany.Test;

public static class ConfigureOneTime {
    private static NHibernate.Cfg.Configuration configuration;
    private static DeleteConfiguration deletion;

    public static NHibernate.Cfg.Configuration GetConfigureNh() {
        if(configuration == null) {
            configuration = Fluently.Configure().Database(SQLiteConfiguration.Standard.InMemory())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserBaseMap>())
                .BuildConfiguration();
            EntityAutoMapping.CreateModel().Configure(configuration);
        }
        return configuration;
    }

    public static DeleteConfiguration GetConfigureDeletion() {
        if(deletion == null) {
            deletion = new DeleteConfiguration(GetConfigureNh());
            DeletionConfiguration.ConfigureDeletion(deletion);
        }
        return deletion;
    }
}
