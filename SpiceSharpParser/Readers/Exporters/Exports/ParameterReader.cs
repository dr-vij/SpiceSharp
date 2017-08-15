﻿using SpiceSharp.Simulations;
using SpiceSharp.Parameters;

namespace SpiceSharp.Parser.Readers.Exports
{
    /// <summary>
    /// This class can read device parameters (eg. "@M1[gm]")
    /// </summary>
    public class ParameterReader : Reader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ParameterReader() : base(StatementType.Export) { }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="netlist">Netlist</param>
        /// <returns></returns>
        public override bool Read(Statement st, Netlist netlist)
        {
            if (st.Name.kind != TokenConstants.REFERENCE)
                return false;
            if (st.Parameters.Count != 1 || (st.Parameters[0].kind != SpiceSharpParserConstants.WORD && st.Parameters[0].kind != SpiceSharpParserConstants.IDENTIFIER))
                return false;
            string component = st.Name.image.Substring(1).ToLower();
            string parameter = st.Parameters[0].image.ToLower();

            var pe = new ParameterExport(component, parameter);
            netlist.Exports.Add(pe);
            Generated = pe;
            return true;
        }
    }

    /// <summary>
    /// A class that can export a device parameter
    /// </summary>
    public class ParameterExport : Export
    {
        /// <summary>
        /// The component
        /// </summary>
        public string Component { get; }

        /// <summary>
        /// The parameter name
        /// </summary>
        public string Parameter { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="component">The component name</param>
        /// <param name="parameter">The parameter name</param>
        public ParameterExport(string component, string parameter)
        {
            Component = component;
            Parameter = parameter;
        }

        /// <summary>
        /// Get the type name
        /// </summary>
        public override string TypeName => "unknown";

        /// <summary>
        /// The display name of the export
        /// </summary>
        public override string Name => "@" + Component + "[" + Parameter + "]";

        /// <summary>
        /// Extract the data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override object Extract(SimulationData data)
        {
            var c = (IParameterized)data.GetObject(Component);
            return c?.Ask(Parameter, data.Circuit);
        }
    }
}
