//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Azure.Documents;
//using Newtonsoft.Json.Linq;

//namespace ServlessEnergyFunctionsApp.Alerting
//{
//    public class DynamicThresholdAlert : Alert
//    {
//        public string MinPropertyName { get; set; }

//        public string MaxPropertyName { get; set; }

//        public override bool IsActive(Document document)
//        {
//            var properties = document.GetPropertyValue<JObject>("properties");

//            if (MinPropertyName != null && properties[MinPropertyName] != null)


//            var minValue = document.GetPropertyValue<string>(MinPropertyName);


//            return (Min.HasValue && readingValue < Min.Value)
//                || (Max.HasValue && readingValue > Max.Value);
//        }

//        public override string ToString()
//        {
//            return $"Threshold(Min={Min}, Max={Max})";
//        }
//    }
//}
