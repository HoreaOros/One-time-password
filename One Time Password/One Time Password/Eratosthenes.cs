using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace One_Time_Password
{
    class Eratosthenes
    {
        public List<int>primeNumbersList=new List<int>();

        public Eratosthenes()
        {
            generateList(10000,30000);
        }

        public void generateList(int startValue,int endValue)
        {
            int num, i, ctr;

            for (num = startValue; num <= endValue; num++)
            {
                ctr = 0;

                for (i = 2; i <= num / 2; i++)
                {
                    if (num % i == 0)
                    {
                        ctr++;
                        break;
                    }
                }

                if (ctr == 0 && num != 1)
                    primeNumbersList.Add(num);
            }
            //System.Windows.MessageBox.Show("done! "+primeNumbersList.Count);
        }
    }
}
