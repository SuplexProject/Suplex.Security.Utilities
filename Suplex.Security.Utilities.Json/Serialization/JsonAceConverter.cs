using System;
using System.Collections.Generic;
using Suplex.Security.AclModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Suplex.Utilities.Serialization
{
    public class JsonAceConverter : JsonConverter
    {
        public override bool CanConvert(Type type)
        {
            return typeof( IAccessControlEntry ).IsAssignableFrom( type );
        }

        public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
        {
            IAccessControlEntry ace = null;

            Dictionary<string, string> props = new Dictionary<string, string>();
            JObject aceJson = JObject.Load( reader );
            foreach( JProperty prop in aceJson.Properties() )
                props[prop.Name] = prop.Value.ToString();

            bool isAuditAce = typeof( IAccessControlEntryAudit ).IsAssignableFrom( type ) ||
                props.ContainsKey( RightFields.Denied );

            if( props.ContainsKey( RightFields.RightData ) )
            {
                props.Remove( RightFields.RightData );
                props[RightFields.RightType] = aceJson[RightFields.RightData].SelectToken( RightFields.RightType ).ToString();
            }

            if( props.ContainsKey( RightFields.RightType ) )
                ace = AccessControlEntryUtilities.MakeAceFromRightType( props[RightFields.RightType], props, isAuditAce );

            return ace;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if( value is IAccessControlEntry ace )
            {
                JObject props = new JObject
                {
                    { nameof( ace.UId ), ace.UId },
                    { RightFields.RightType, ace.RightData.RightType.AssemblyQualifiedName },
                    { RightFields.Right, ace.RightData.Name },
                    { nameof( ace.Allowed ), ace.Allowed }
                };

                if( ace is IAccessControlEntryAudit auditace )
                    props.Add( nameof( auditace.Denied ), auditace.Denied );

                props.Add( nameof( ace.Inheritable ), ace.Inheritable );

                if( ace.InheritedFrom.HasValue )
                    props.Add( nameof( ace.InheritedFrom ), ace.InheritedFrom );

                if( ace.TrusteeUId.HasValue )
                    props.Add( nameof( ace.TrusteeUId ), ace.TrusteeUId );

                props.WriteTo( writer );
            }
        }
    }
}