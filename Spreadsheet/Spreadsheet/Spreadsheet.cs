using System;
using System.Collections.Generic;
using Formulas;
using Dependencies;
using System.Text.RegularExpressions;
using System.IO;

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
        /// <summary>
        /// All cells in the spreadsheet.
        /// </summary>
        private CellContainer cells;

        /// <summary>
        /// All dependencies (between cells) in the spreadsheet.
        /// </summary>
        private DependencyGraph dependencies;

        /// <summary>
        /// IsValid is used to determine whether cell names are valid, and
        /// 
        /// A string is a valid cell name if and only if (1) s consists of one or more letters, 
        /// followed by a non-zero digit, followed by zero or more digits AND (2) the C#
        /// expression IsValid.IsMatch(s.ToUpper()) is true.
        /// 
        /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names, so long as they also
        /// are accepted by IsValid.  On the other hand, "Z", "X07", and "hello" are not valid cell 
        /// names, regardless of IsValid.
        /// </summary>
        private Regex IsValid

        // todo: finish
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression is provided as the parameter.
        /// </summary>
        public Spreadsheet(Regex isValid)
        {

        }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression accepts every string.
        /// </summary>
        public Spreadsheet() : this(new Regex(".*", RegexOptions.Singleline))
        {
            cells = new CellContainer();
            dependencies = new DependencyGraph();
        }

        // todo: delete me?
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

        // todo: delete me?
        /// <summary>
        /// Helper method that returns whether (name) is a valid cell name.
        /// </summary>
        private bool IsValidCellName(string name)
        {
            if (name != null)
            {
                return Regex.IsMatch(name, validCellNamePattern);
            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (string s in cells.GetNamesOfAllNonemptyCells())
            {
                yield return s;
            }
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the IsValid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            // todo: implement me
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (IsValidCellName(name))
            {
                return cells.GetCellContents(name);
            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            // todo: implement me
            return new object();
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<String> SetContentsOfCell(string name, string content)
        {
            // todo: check if comment on next line is true (and then remove it)
            if (IsValidCellName(name)) // probably don't need else, because this method should take care of it
            {
                // if content is a number
                if (Double.TryParse(content, out double number))
                {
                    SetCellContents(name, number);
                }

                // if content is a formula
                else if (content[0] == '=')
                {
                    Formula formula = new Formula(content.Remove(0, 1), s => s.ToUpper(), );

                    SetCellContents(name, )
                }
            }
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
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (IsValidCellName(name))
            {
                cells.SetCellContents(name, number);

                return GetAllDependents(name);
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
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if (text != null)
            {
                if (IsValidCellName(name))
                {
                    cells.SetCellContents(name, text);

                    return GetAllDependents(name);
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
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            HashSet<string> vars = (HashSet<string>)formula.GetVariables();
            foreach (string variable in vars)
            {
                if (!IsValidCellName(variable))
                {
                    throw new InvalidNameException();
                }
            }

            if (IsValidCellName(name))
            {
                dependencies.ReplaceDependees(name, vars);
                HashSet<string> result = (HashSet<string>)GetAllDependents(name);

                cells.SetCellContents(name, formula);
                return result;
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
            return dependencies.GetDependents(name);
        }

        /// <summary>
        /// Helper method finds the names of all direct dependents of string (start), then adds
        /// them to a HashSet, which is returned.
        /// 
        /// Useful to avoid lots of casting to (HashSet) from IEnumerable or ISet.
        /// </summary>
        private HashSet<string> GetHashSetOfDirectDependents(string start)
        {
            HashSet<string> hashSet = new HashSet<string>();
            foreach (string dependent in GetDirectDependents(start))
            {
                hashSet.Add(dependent);
            }
            return hashSet;
        }

        /// <summary>
        /// Recursive helper method for GetAllDependents.
        /// Retrieves all indirect dependents from a set of direct dependents (directDependents).
        /// </summary>
        private HashSet<string> GetIndirectDependents(HashSet<string> directDependents)
        {
            HashSet<string> allIndirectDependents = new HashSet<string>();
            foreach (string directDependent in directDependents)
            {
                HashSet<string> nextLayerIndirectDependents = GetHashSetOfDirectDependents(directDependent);
                foreach (string indirectDependent in nextLayerIndirectDependents)
                {
                    HashSet<string> deeper = GetIndirectDependents(nextLayerIndirectDependents);
                    allIndirectDependents.Add(indirectDependent);

                    foreach (string s in deeper)
                    {
                        allIndirectDependents.Add(s);
                    }
                }
                allIndirectDependents.Add(directDependent);
            }
            return allIndirectDependents;
        }
        
        /// <summary>
        /// Returns an ISet containing all dependents of cell (name), including cell (name).
        /// </summary>
        private ISet<string> GetAllDependents(string name)
        {
            // GetCellsToRecalculate will throw a CircularException if result contains
            // a circular dependency.
            GetCellsToRecalculate(name);

            HashSet<string> direct = GetHashSetOfDirectDependents(name);
            direct.Add(name);
            return GetIndirectDependents(direct);
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
        /// 
        /// Note: set calls SetContent to make sure the dynamic parameter is always an allowable type.
        /// </summary>
        public dynamic Contents
        {
            get { return _contents; }
            set { SetContent(value); }
        }
        private dynamic _contents;

        /// <summary>
        /// Initializes this cell, which will be named (name) and contain content (content).
        /// 
        /// Note: content will necessarily, by virtue of SetCellContents, be of type string, double,
        /// or Formula. However, this constructor still calls SetContent to make sure the dynamic
        /// parameter is always an allowable type.
        /// </summary>
        public Cell(dynamic content)
        {
            SetContent(content);
        }

        private void SetContent(dynamic content)
        {
            if (content is double || content is string || content is Formula)
            {
                _contents = content;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
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
        /// <summary>
        /// A dictionary containing all of the CellContainer's (and thus, the Spreadsheet's)
        /// Cells. Its keys are indexed by a string with each Cell's name.
        /// </summary>
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
        /// 
        /// This is a wrapper method which will call the correct SetCellContents, given the type
        /// of content.
        /// </summary>
        public void SetCellContents(string name, dynamic content)
        {
            if (content is double || content is string || content is Formula)
            {
                SetCellContents(name, content);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Creates a new cell in cells with name (name) and content of double (content).
        /// </summary>
        private void SetCellContents(string name, double content)
        {
            if (cells.ContainsKey(name))
            {
                cells[name].Contents = content;
            }
            else
            {
                cells[name] = new Cell(content);
            }
        }

        /// <summary>
        /// Creates a new cell in cells with name (name) and content of string (content).
        /// </summary>
        private void SetCellContents(string name, string content)
        {
            if (content == "")
            {
                cells.Remove(name);
            }
            else
            {
                if (cells.ContainsKey(name))
                {
                    cells[name].Contents = content;
                }
                else
                {
                    cells[name] = new Cell(content);
                }
            }
        }

        /// <summary>
        /// Creates a new cell in cells with name (name) and content of Formula (content).
        /// </summary>
        private void SetCellContents(string name, Formula content)
        {
            if (cells.ContainsKey(name))
            {
                cells[name].Contents = content;
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
        public object GetCellContents(string name)
        {
            if (name != null)
            {
                if (cells.ContainsKey(name))
                {
                    return cells[name].Contents;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// Retrieves the name of all (non-empty) Cells in the Cellcontainer.
        /// Actually returns all Cells, because non-empty cells are only abstractly represented,
        /// and not actually contained.
        /// </summary>
        public IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (string s in cells.Keys)
            {
                if (cells[s].Contents.GetType() == typeof(string))
                {
                    if (cells[s].Contents != "")
                    {
                        yield return s;
                    }
                }
                else
                {
                    yield return s;
                }
            }
        }
    }
}
