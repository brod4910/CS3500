using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formulas;
using System.Threading.Tasks;

namespace SS
{
    class Cell
    {
        private object contents;

        public Cell()
        {
            this.contents = null;
        }

        public Cell(Formula contents)
        {
            this.contents = contents;
        }

        public Cell(Double contents)
        {
            this.contents = contents;
        }

        public Cell(String contents)
        {
            this.contents = contents;
        }

        public object getContents
        {
            get { return contents; }
        }

        public void setContents(object contents)
        {
            this.contents = contents;
        }
    }
}
