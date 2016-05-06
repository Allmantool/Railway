using System.ComponentModel.DataAnnotations;

namespace NaftanRailway.WebUI.Infrastructure.ValidationAttributes {
    //custom validation attr. Then can use in metadata validation
    public class MustBeTrueAttribute:ValidationAttribute {
        //The validation logic is simple; a value is valid if it is a bool that has a value of true
        public override bool IsValid(object value) {
            return value is bool && (bool)value;
        }
    }
}