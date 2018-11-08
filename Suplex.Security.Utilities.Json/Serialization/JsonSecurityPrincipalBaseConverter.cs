using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Suplex.Security.Principal;
using System;

namespace Suplex.Utilities.Serialization
{
    public class JsonSecurityPrincipalBaseConverter: JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof( SecurityPrincipalBase ));
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //https://stackoverflow.com/questions/34185295/handling-null-objects-in-custom-jsonconverters-readjson-method
            if( reader.TokenType == JsonToken.Null ) return null;

            JObject jo = JObject.Load( reader );
            bool isUser = (bool)jo["IsUser"];

            if( isUser )
                return jo.ToObject<User>( serializer );
            else
                return jo.ToObject<Group>( serializer );

            //not sure which is better
            //SecurityPrincipalBase target = null;
            //if( isUser )
            //    target = new User();
            //else
            //    target = new Group();
            //serializer.Populate(jo.CreateReader(), target);
            //return target;
        }
        public override bool CanWrite
        {
            get { return false; }
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
