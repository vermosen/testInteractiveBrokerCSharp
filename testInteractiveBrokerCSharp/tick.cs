using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testInteractiveBrokerCSharp
{
    public class tick
    {

        public String  type     = "";
        public decimal value    = 0 ;
        public String date      = System.DateTime.Now.ToString("yyyy-MM-dd");
        public String time      = System.DateTime.Now.ToString("H:mm:ss.ss");
        public String symbol    = "";
        public Int32 index      = 0 ;

        public tick(String type, decimal value, String symbol, Int32 index)
        {
            this.type   = type  ;
            this.value  = value ;
            this.symbol = symbol;
            this.index  = index ;
        }

        public tick() {}

        public String insertStr()
        {
            String output = "insert into ticks (idticks,symbol,date,time,value,type) values (" +
                                index +
                                ",'" + symbol +
                                "', DATE('" + date +
                                "'),TIME('" + time + "')," +
                                String.Format(System.Globalization.CultureInfo.GetCultureInfo("us-US"), "{0:0.0}", value) +      // to get the us format
                                ",'" + type + "')"; // turn into global settings ?

            return output;
        }
    }
}
