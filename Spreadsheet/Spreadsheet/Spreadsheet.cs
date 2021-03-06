﻿using System;
using System.Collections.Generic;
using Formulas;
using Dependencies;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.Schema;

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
        private Regex IsValid;

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
            IsValid = isValid;
            cells = new CellContainer();
            dependencies = new DependencyGraph();
            Changed = false;
        }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression accepts every string.
        /// </summary>
        public Spreadsheet() : this(new Regex(".*", RegexOptions.Singleline))
        {
        }

        /// <summary>
        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        ///
        /// If there's a problem reading source, throws an IOException.
        ///
        /// Else if the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  
        ///
        /// Else if the IsValid string contained in source is not a valid C# regular expression, throws
        /// a SpreadsheetReadException.  (If the exception is not thrown, this regex is referred to
        /// below as oldIsValid.)
        ///
        /// Else if there is a duplicate cell name in the source, throws a SpreadsheetReadException.
        /// (Two cell names are duplicates if they are identical after being converted to upper case.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a 
        /// SpreadsheetReadException.  (Use oldIsValid in place of IsValid in the definition of 
        /// cell name validity.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a
        /// SpreadsheetVersionException.  (Use newIsValid in place of IsValid in the definition of
        /// cell name validity.)
        ///
        /// Else if there's a formula that causes a circular dependency, throws a SpreadsheetReadException. 
        ///
        /// Else, create a Spreadsheet that is a duplicate of the one encoded in source except that
        /// the new Spreadsheet's IsValid regular expression should be newIsValid.
        /// </summary>
        public Spreadsheet(TextReader source, Regex newIsValid) : this(newIsValid)
        {
            try
            {
                // schema validation
                XmlReaderSettings settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
                settings.Schemas.Add(null, "Spreadsheet.xsd");
                settings.ValidationEventHandler += (object o, ValidationEventArgs v) =>
                {
                    throw new SpreadsheetReadException(v.Message);
                };

                foreach (Tuple<string, string> cell in ReadInSpreadsheet(source, settings, out Regex oldIsValid))
                {
                    try
                    {
                        string name = cell.Item1;

                        if (!oldIsValid.IsMatch(name))
                        {
                            throw new SpreadsheetReadException
                                ("Source contains cell that does not match source's IsValid.");
                        }
                        if ((string)GetCellContents(name) != "")
                        {
                            throw new SpreadsheetReadException("Source contains duplicate cell names.");
                        }
                    }
                    finally
                    {
                        SetContentsOfCell(cell.Item1, cell.Item2);
                    }
                }
            }
            catch (InvalidNameException)
            {
                throw new SpreadsheetVersionException("Source contains cell that does not match newIsValid.");
            }
            catch (ArgumentException)
            {
                throw new SpreadsheetReadException("Source's IsValid is not a valid C# regular expression.");
            }
            catch (XmlException)
            {
                throw new IOException();
            }
            catch (XmlSchemaValidationException)
            {
                throw new SpreadsheetReadException("Source does not meet schema definition.");
            }
        }

        /// <summary>
        /// Private helper method goes inside public wrapper method. Reads contents of (source)
        /// according to (settings), and raises exceptions if the contents of (source) do not
        /// meet specifications. Returns Regex IsValid from (source).
        /// </summary>
        private HashSet<Tuple<string, string>> ReadInSpreadsheet
            (TextReader source, XmlReaderSettings settings, out Regex oldIsValid)
        {
            HashSet<Tuple<string, string>> cells = new HashSet<Tuple<string, string>>();
            oldIsValid = null;

            using (XmlReader reader = XmlReader.Create(source, settings))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.AttributeCount == 1)
                        {
                            oldIsValid = new Regex(reader.GetAttribute("IsValid"));
                        }

                        if (reader.AttributeCount == 2)
                        {
                            //SetContentsOfCell(reader.GetAttribute("name"), reader.GetAttribute("contents"));
                            //yield return new Tuple<string, string>
                            //    (reader.GetAttribute("name"), reader.GetAttribute("contents"));
                            cells.Add(new Tuple<string, string>
                                (reader.GetAttribute("name"), reader.GetAttribute("contents")));
                        }
                    }
                }
            }

            source.Close();
            return cells;
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
            try
            {
                using (XmlWriter writer = XmlWriter.Create(dest))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("IsValid", IsValid.ToString());

                    foreach (string name in cells.GetNamesOfAllNonemptyCells())
                    {
                        object contents = cells.GetCellContents(name);

                        if (contents is string)
                        {
                            contents = (string)contents;
                        }
                        else if (contents is double)
                        {
                            contents = contents.ToString();
                        }
                        else
                        {
                            contents = "=" + contents.ToString();
                        }

                        writer.WriteStartElement("cell");
                        writer.WriteAttributeString("contents", (string)contents);
                        writer.WriteAttributeString("name", name);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                dest.Close();

                // todo: "if there are any problems..." -- what kind of problems?

                Changed = false;
            }
            catch
            {
                throw new IOException();
            }
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
        /// Helper method that returns whether (name) is a valid cell name.
        /// </summary>
        private bool IsValidCellName(string name)
        {
            if (name != null)
            {
                return Regex.IsMatch(name.ToUpper(), validCellNamePattern) && IsValid.IsMatch(name.ToUpper());
            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (!IsValidCellName(name))
            {
                throw new InvalidNameException();
            }

            name = name.ToUpper();
            return cells.GetCellContents(name);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (!IsValidCellName(name))
            {
                throw new InvalidNameException();
            }

            name = name.ToUpper();
            return cells.GetCellValue(name);
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
            if (content == null)
            {
                throw new ArgumentNullException();
            }

            if (!IsValidCellName(name))
            {
                throw new InvalidNameException();
            }
            
            name = name.ToUpper();
            HashSet<string> result = new HashSet<string>();

            if (content.Length > 0)
            {
                if (Double.TryParse(content, out double number))
                {
                    result = (HashSet<string>)SetCellContents(name, number);
                }
                else if (content[0] == '=')
                {
                    content = content.Remove(0, 1);
                    content = content.ToUpper();

                    Formula formula = new Formula(content, s => s.ToUpper(), IsValidCellName);
                    result = (HashSet<string>)SetCellContents(name, formula);
                }
                else
                {
                    result = (HashSet<string>)SetCellContents(name, content);
                }
            }
            else
            {
                dependencies.ReplaceDependents(name, new HashSet<string>());
                result = (HashSet<string>)SetCellContents(name, content);
            }

            Changed = true;

            foreach (string cellName in GetCellsToRecalculate(name))
            {
                cells.ReevaluateCell(cellName);
            }

            return result;
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
            cells.SetCellContents(name, number);
            return GetAllDependents(name);
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
            cells.SetCellContents(name, text);
            return GetAllDependents(name);
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

            dependencies.ReplaceDependees(name, vars);
            HashSet<string> result = (HashSet<string>)GetAllDependents(name);
            cells.SetCellContents(name, formula);
            return result;
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
        public void SetCellContents(string name, object content)
        {
            if (content is double || content is string || content is Formula)
            {
                if (content is double)
                {
                    SetCellContents(name, (double)content);
                }
                else if (content is string)
                {
                    SetCellContents(name, (string)content);
                }
                else // content is Formula
                {
                    SetCellContents(name, (Formula)content);
                }
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
                cells[name] = new Cell(content, TryLookup);
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
                    cells[name] = new Cell(content, TryLookup);
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
                cells[name] = new Cell(content, TryLookup);
            }
        }

        /// <summary>
        /// Reevaluates cell (name) whose value may have changed.
        /// </summary>
        public void ReevaluateCell(string name)
        {
            if (cells.ContainsKey(name))
            {
                cells[name].Evaluate(TryLookup);
            }
        }

        /// <summary>
        /// Returns the content of cell (name).
        /// 
        /// If cell (name) isn't in cells yet, returns default value "".
        /// </summary>
        public object GetCellContents(string name)
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

        /// <summary>
        /// Returns the value of cell (name).
        /// 
        /// If cell (name) isn't in cells yet, returns default content "".
        /// </summary>
        public object GetCellValue(string name)
        {
            if (cells.ContainsKey(name))
            {
                return cells[name].Value;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Returns the value of cell (name), if (name).Value is of type double.
        /// 
        /// If cell (name) is not in cells, or if its value is not a double, throws an UndefinedVariableException.
        /// </summary>
        public double TryLookup(string name)
        {
            if (!cells.ContainsKey(name))
            {
                throw new UndefinedVariableException(name);
            }
            else
            {
                if (cells[name].Value is double)
                {
                    return (double)cells[name].Value;
                }
            }

            throw new UndefinedVariableException(name);
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
                yield return s;
            }
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
        /// The cell's content, which is initialized in constructor, but may be indirectly modified
        /// by CellContainer.SetCellContent.
        /// 
        /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
        /// contents is an empty string, we say that the cell is empty.
        /// 
        /// Note: set calls SetContent to make sure the object parameter is always an allowable type.
        /// </summary>
        public dynamic Contents
        {
            get { return _contents; }
            set { SetContent(value); }
        }
        private dynamic _contents;

        /// <summary>
        /// The cell's value, which is initially evaluated at construction, but may be indirectly
        /// re-evaluated by CellContainer.ReevaluateCell.
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
        /// </summary>
        public dynamic Value
        {
            get { return _value; }
        }
        private dynamic _value;

        /// <summary>
        /// Initializes this cell, which will be named (name) and contain content (content).
        /// 
        /// Note: content will necessarily, by virtue of SetCellContents, be of type string, double,
        /// or Formula. However, this constructor still calls SetContent to make sure the object
        /// parameter is always an allowable type.
        /// </summary>
        public Cell(dynamic content, Formula.Lookup lookup)
        {
            SetContent(content);
            Evaluate(lookup);
        }

        /// <summary>
        /// Helper method for Contents.Set sets _content to (content).
        /// 
        /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
        /// contents is an empty string, we say that the cell is empty.
        /// </summary>
        private void SetContent(dynamic content)
        {
            if (content is string || content is double || content is Formula)
            {
                _contents = content;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Helper method that evaluates the cell's _value, given its content.
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
        /// </summary>
        public void Evaluate(Formula.Lookup lookup)
        {
            if (_contents is Formula)
            {
                try
                {
                    _value = _contents.Evaluate(lookup);
                }
                catch (FormulaEvaluationException)
                {
                    _value = new FormulaError("Formula could not be evaluated.");
                }
            }
            else if (_contents is string || _contents is double)
            {
                _value = _contents;
            }
            else
            {
                throw new Microsoft.CSharp.RuntimeBinder.RuntimeBinderException();
            }
        }
    }
}
