using UnityEngine;
using Zenject;
using Newtonsoft.Json;

namespace SBaier.Master
{
    public class SerializationSettingsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MaxDepth = 10;
            settings.FloatFormatHandling = FloatFormatHandling.String;
            settings.FloatParseHandling = FloatParseHandling.Double;
            //settings.Formatting = Formatting.Indented;
            //settings.ConstructorHandling = ConstructorHandling.Default;
            //settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
            //settings.TypeNameHandling = TypeNameHandling.All;
            //settings.PreserveReferencesHandling = PreserveReferencesHandling.All;
            //settings.NullValueHandling = NullValueHandling.Include;
            //settings.ObjectCreationHandling = ObjectCreationHandling.Reuse;
            //settings.MissingMemberHandling = MissingMemberHandling.Error;
            //settings.ReferenceLoopHandling = ReferenceLoopHandling.Error;
            Container.Bind<JsonSerializerSettings>().FromInstance(settings).AsSingle();
        }
    }
}