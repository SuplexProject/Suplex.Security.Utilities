using System;
using System.Collections.Generic;

using Suplex.Security.AclModel;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Suplex.Utilities.Serialization
{
    public class YamlAceConverterConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return typeof( IAccessControlEntryConverter ).IsAssignableFrom( type );
        }

        public object ReadYaml(IParser parser, Type type)
        {
            IAccessControlEntryConverter ace = null;

            if( typeof( IAccessControlEntryConverter ).IsAssignableFrom( type ) && parser.Current.GetType() == typeof( MappingStart ) )
            {
                parser.MoveNext(); // skip the sequence start

                Dictionary<string, string> props = new Dictionary<string, string>();
                while( parser.Current.GetType() != typeof( MappingEnd ) )
                {
                    string prop = ((Scalar)parser.Current).Value;
                    parser.MoveNext();
                    string value = ((Scalar)parser.Current).Value;
                    parser.MoveNext();

                    props[prop] = value;
                }
                parser.MoveNext();

                if( props.ContainsKey( RightFields.SourceRightType )&& props.ContainsKey( RightFields.TargetRightType ) )
                    ace = AccessControlEntryConverterUtilities.MakeAceConverterFromRightType( props[RightFields.SourceRightType], props[RightFields.TargetRightType], props );
            }

            return ace;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            emitter.Emit( new MappingStart( null, null, false, MappingStyle.Block ) );

            if( value is IAccessControlEntryConverter cv )
            {
                emitter.Emit( new Scalar( null, nameof( cv.UId ) ) );
                emitter.Emit( new Scalar( null, cv.UId.ToString() ) );

                emitter.Emit( new Scalar( null, RightFields.SourceRight ) );
                emitter.Emit( new Scalar( null, cv.SourceRightName ) );
                emitter.Emit( new Scalar( null, RightFields.SourceRightType ) );
                emitter.Emit( new Scalar( null, cv.SourceRightType.AssemblyQualifiedName ) );

                emitter.Emit( new Scalar( null, RightFields.TargetRight ) );
                emitter.Emit( new Scalar( null, cv.TargetRightName ) );
                emitter.Emit( new Scalar( null, RightFields.TargetRightType ) );
                emitter.Emit( new Scalar( null, cv.TargetRightType.AssemblyQualifiedName ) );

                emitter.Emit( new Scalar( null, nameof( cv.Inheritable ) ) );
                emitter.Emit( new Scalar( null, cv.Inheritable.ToString() ) );
            }

            emitter.Emit( new MappingEnd() );
        }
    }
}