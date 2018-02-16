using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using Dependencies;
using System.Text.RegularExpressions;

namespace SS
{
    /// <summary>
    /// A spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string s is a valid cell name if and only if it consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names.  On the other hand, 
    /// "Z", "X07", and "hello" are not valid cell names.
    /// 
    /// A spreadsheet contains a unique cell corresponding to each possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important, and it is important that you understand the distinction and use
    /// the right term when writing code, writing comments, and asking questions.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In an empty spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  The value 
    /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
    /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
    /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
    /// is a double, as specified in Formula.Evaluate.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        // todo: make sure everything has Summary tags
        // todo: implement all abstract methods

        // todo: figure out what else (beyond "GDD") needs to be done regarding GCTR/Visit)
        // todo: remove all comments (including todos)

        /// <summary>
        /// All cells in the spreadsheet.
        /// </summary>
        private CellContainer cells;

        /// <summary>
        /// All dependencies (between cells) in the spreadsheet.
        /// </summary>
        private DependencyGraph dependencies;

        /// <summary>
        /// Constructs an empty Spreadsheet.
        /// </summary>
        public Spreadsheet()
        {
            cells = new CellContainer();
            dependencies = new DependencyGraph();
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            // todo: finish this method
            return new HashSet<string> { };
        }

        /// <summary>
        /// A Regex pattern that will be used to determine whether a given cell name is valid.
        /// 
        /// Specification for valid cell name:
        /// "A string s is a valid cell name if and only if it consists of one or more letters, 
        /// followed by a non-zero digit, followed by zero or more digits.
        /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names.  On the other hand, 
        /// "Z", "X07", and "hello" are not valid cell names."
        /// </summary>
        private const string validCellNamePattern = "^[a-zA-Z]+[1-9][0-9]*$";

        /// <summary>
        /// Returns whether (name) is a valid cell name.
        /// </summary>
        private bool IsValidCellName(string name)
        {
            return Regex.IsMatch(name, validCellNamePattern);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            return cells.GetCellContent(name);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, double number)
        {
            if (IsValidCellName(name))
            {
                cells.SetCellContent(name, number);

                // todo: return correct thing here
                return new HashSet<string> { };
            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, string text)
        {
            if (text != null)
            {
                if (IsValidCellName(name))
                {
                    cells.SetCellContent(name, text);

                    // todo: return correct thing here
                    return new HashSet<string> { };
                }
                else
                {
                    throw new InvalidNameException();
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Requires that all of the variables in formula are valid cell names.
        /// 
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            foreach (string variable in formula.GetVariables())
            {
                if (!IsValidCellName(variable))
                {
                    throw new InvalidNameException();
                }
            }
            // todo: CircularException detection
            if (IsValidCellName(name))
            {
                cells.SetCellContent(name, formula);

                // todo: return correct thing here
                return new HashSet<string> { };
            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return new HashSet<string> { };
        }

        private IEnumerable<string> GetIndirectDependents(string name)
        {
            return (HashSet<string>)GetIndirectDependents(GetDirectDependents(name));
        }

        private IEnumerable<string> GetIndirectDependents(IEnumerable<string> names)
        {
            HashSet<string> result = new HashSet<string>();
            foreach (string s in names)
            {
                foreach (string t in GetIndirectDependents(GetDirectDependents(s)))
                {
                    result.Add(t);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// A cell in a spreadsheet. Each cell has a contents and a value.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// If a cell's contents is a string, its value is that string.
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  The value 
    /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
    /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
    /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
    /// is a double, as specified in Formula.Evaluate.
    /// </summary>
    internal class Cell
    {
        /// <summary>
        /// The cell's content. Initialized in constructor.
        /// 
        /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
        /// contents is an empty string, we say that the cell is empty.
        /// </summary>
        public object Content
        {
            get { return _content; }
            set { _content = value; }
        }
        private object _content;

        /// <summary>
        /// Initializes this cell, which will be named (name) and contain content (content).
        /// Note: content will necessarily, by virtue of SetCellContents, be of type string, double, or Formula.
        /// </summary>
        public Cell(object content)
        {
            _content = content;
        }
    }

    /// <summary>
    /// Contains all cells in a spreadsheet. Built to fulfull the specification that "a spreadsheet
    /// contains a unique cell corresponding to each possible cell name", which is, naturally,
    /// computationally impossible with direct representation; thus, I will abstract one level
    /// of representation to fulfill that specification as well as is possible computationally.
    /// </summary>
    internal class CellContainer
    {
        private Dictionary<string, Cell> cells;
        
        /// <summary>
        /// Initializes an empty CellContainer.
        /// </summary>
        public CellContainer()
        {
            cells = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Creates a new cell in cells with name (name) and content (content).
        /// By virtue of SetCellContents, (content) will not be of type string, double, or Formula.
        /// </summary>
        public void SetCellContent(string name, object content)
        {
            if (cells.ContainsKey(name))
            {
                cells[name].Content = content;
            }
            else
            {
                cells[name] = new Cell(content);
            }
        }

        /// <summary>
        /// Returns the content of cell (name). If cell (name) isn't in cells yet, returns default
        /// value "".
        /// </summary>
        public object GetCellContent(string name)
        {
            if (cells.ContainsKey(name))
            {
                return cells[name].Content;
            }
            else
            {
                return "";
            }
        }
    }
}
