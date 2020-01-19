using System;
using System.Collections.Generic;

namespace CumulativeData
{
    public class CumulativeDataRow  
    {
        public string Product { get; private  set; }
        public List<double> Increments { get; private set; }

        public CumulativeDataRow(string product)
        {
            Product = product;
            Increments = new List<double>();
        }

        public void AddIncrement(double increment)
        {
            Increments.Add(increment);
        }

        public override string ToString()
        {
            return Product+","+ String.Join(",", Increments);
        }
    }
}