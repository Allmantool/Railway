using System;
using System.ComponentModel;

namespace NaftanRailway.WebUI.Infrastructure.TypeConversion {
    //[TypeConverter(typeof(SomeClass))]
    /// <summary>
    /// require for custom parameters binding (ubiquitous for web form, wpf, web api etc )
    /// Model binding is more specific solution (can work with httpContext) for model conversion rather then type conversion
    /// </summary>
    public class Demo : TypeConverter {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return base.CanConvertTo(context, destinationType);
        }
    }
}