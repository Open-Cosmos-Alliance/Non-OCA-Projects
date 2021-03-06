﻿using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Reflection.Emit;


namespace Cosmos.Assembler
{
    public static class MethodInfoLabelGenerator
    {
      public static uint LabelCount {
        get;
        private set;
      }

        public static string GenerateLabelName( MethodBase aMethod )
        {
            string xResult = DataMember.FilterStringForIncorrectChars( GenerateFullName( aMethod ) );
            xResult = GenerateLabelFromFullName( xResult );
            LabelCount++;
            return xResult;
        }

        public static string GenerateLabelFromFullName( string xResult )
        {
            if( xResult.Length > 245 )
            {
                using( var xHash = MD5.Create() )
                {
                    xResult = xHash.ComputeHash(
                        Encoding.Default.GetBytes( xResult ) ).Aggregate( "_", ( r, x ) => r + x.ToString( "X2" ) );
                }
            }
            return xResult;
        }


        //public static string GenerateLabelName(MethodBase aMethod)
        //{
        //    return GenerateLabelNameInt(aMethod).ToString();
        //}
        //            unsigned long hash(char *name)
        //{
        // unsigned long h = 0, g;

        // while ( *name ) {
        //  h = ( h << 4 ) + *name++;
        //  if ( g = h & 0xF0000000 )
        //  h ^= g >> 24;
        //  h &= ~g;
        // }

        // return h;
        //}



        public static string GetFullName( Type aType )
        {
            if( aType.IsGenericParameter )
            {
              return aType.FullName;
            }
            var xSB = new StringBuilder();
            if( aType.IsArray )
            {
                xSB.Append( GetFullName( aType.GetElementType() ) );
                xSB.Append( "[" );
                int xRank = aType.GetArrayRank();
                while( xRank > 1 )
                {
                    xSB.Append( "," );
                    xRank--;
                }
                xSB.Append( "]" );
                return xSB.ToString();
            }
            if( aType.IsByRef && aType.HasElementType )
            {
                return "&" + GetFullName( aType.GetElementType() );
            }
            if( aType.IsGenericType && !aType.IsGenericTypeDefinition )
            {
                xSB.Append( GetFullName(aType.GetGenericTypeDefinition()));
            }
            else
            {
              xSB.Append(aType.FullName);
            }
            if( aType.IsGenericType )
            {
                xSB.Append( "<" );
                var xArgs = aType.GetGenericArguments();
                for( int i = 0; i < xArgs.Length - 1; i++ )
                {
                    xSB.Append( GetFullName( xArgs[ i ] ) );
                    xSB.Append( ", " );
                }
                xSB.Append( GetFullName( xArgs.Last() ) );
                xSB.Append( ">" );
            }
            //xSB.Append(", ");
            //xSB.Append(aType.Assembly.FullName);
            return xSB.ToString();
        }

        public static string GenerateFullName( MethodBase aMethod )
        {
            if( aMethod == null )
            {
                throw new ArgumentNullException( "aMethod" );
            }
            var xBuilder = new StringBuilder( 256 );
            var xParts = aMethod.ToString().Split( ' ' );
            var xParts2 = xParts.Skip( 1 ).ToArray();
            var xMethodInfo = aMethod as System.Reflection.MethodInfo;
            if( xMethodInfo != null )
            {
                xBuilder.Append( GetFullName( xMethodInfo.ReturnType ) );
            }
            else
            {
                var xCtor = aMethod as ConstructorInfo;
                if( xCtor != null )
                {
                    xBuilder.Append( typeof( void ).FullName );
                }
                else
                {
                    xBuilder.Append( xParts[ 0 ] );
                }
            }
            xBuilder.Append( "  " );
            if (aMethod.DeclaringType != null)
            {
                xBuilder.Append(GetFullName(aMethod.DeclaringType));
            }
            else
            {
                xBuilder.Append("dynamic_method");
            }
            xBuilder.Append( "." );
            xBuilder.Append( aMethod.Name );
            if( aMethod.IsGenericMethod || aMethod.IsGenericMethodDefinition )
            {
                var xGenArgs = aMethod.GetGenericArguments();
                if( xGenArgs.Length > 0 )
                {
                    xBuilder.Append( "<" );
                    for( int i = 0; i < xGenArgs.Length - 1; i++ )
                    {
                        xBuilder.Append( GetFullName( xGenArgs[ i ] ) );
                        xBuilder.Append( ", " );
                    }
                    xBuilder.Append( GetFullName( xGenArgs.Last() ) );
                    xBuilder.Append( ">" );
                }
            }
            xBuilder.Append( "(" );
            var xParams = aMethod.GetParameters();
            for( var i = 0; i < xParams.Length; i++ )
            {
                if( xParams[ i ].Name == "aThis" && i == 0 )
                {
                    continue;
                }
                xBuilder.Append( GetFullName( xParams[ i ].ParameterType ) );
                if( i < ( xParams.Length - 1 ) )
                {
                    xBuilder.Append( ", " );
                }
            }
            xBuilder.Append( ")" );
            return String.Intern( xBuilder.ToString() );
        }

        public static string GetFullName( FieldInfo aField )
        {
            return GetFullName( aField.FieldType ) + " " + GetFullName( aField.DeclaringType ) + "." + aField.Name;
            //var xSB = new StringBuilder(aField.FieldType.FullName.Length + 1 + aField.DeclaringType.FullName.Length + 1 + aField.Name);
            //xSB.Append(aField.FieldType.FullName);
            //xSB.Append(" ");
            //xSB.Append(aField.DeclaringType.FullName);
            //xSB.Append(".");
            //xSB.Append(aField.Name);
            //return String.Intern(xSB.ToString());
        }
    }
}
