// ================================================================================================
// Commands.cs
//
// Created: 2009.02.10, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VSXtra.Shell
{
  public class CommandCollection: IEnumerable<Command>
  {
    #region Private fields

    private readonly List<Command> _CommandList;

    #endregion

    #region Lifecycle methods

    public CommandCollection()
    {
      _CommandList = new List<Command>(AllVsIdeCommands);
    }

    public CommandCollection(IEnumerable<Command> commands)
    {
      _CommandList = new List<Command>(commands);
    }

    public CommandCollection(Func<Command, bool> commandFilter)
    {
      _CommandList = new List<Command>(AllVsIdeCommands.Where(commandFilter));
    }

    public CommandCollection(IEnumerable<Command> commands, Func<Command, bool> commandFilter)
    {
      _CommandList = new List<Command>(commands.Where(commandFilter));
    }

    #endregion

    #region Public Properties

    public Command ByExactName(string name, bool ignoreCase)
    {
      return AllVsIdeCommands.First(c => String.Compare(name, c.Name, ignoreCase) == 0);
    }

    public Command ByExactName(string name)
    {
      return ByExactName(name, true);
    }

    #endregion

    #region Private methods

    private static IEnumerable<Command> AllVsIdeCommands
    {
      get
      {
        foreach (var cmd in VsIde.DteInstance.Commands)
        {
          var dteCommand = cmd as EnvDTE.Command;
          if (dteCommand != null) yield return new Command(dteCommand);
        }
      }
    }

    #endregion


    #region Implementation of IEnumerable

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    IEnumerator<Command> IEnumerable<Command>.GetEnumerator()
    {
      return _CommandList.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _CommandList.GetEnumerator();
    }

    #endregion
  }

  public class Command
  {
    private EnvDTE.Command _DteCommand;

    public Command(EnvDTE.Command cmd)
    {
      _DteCommand = cmd;
    }

    public Guid Guid
    {
      get { return new Guid(_DteCommand.Guid); }
    }

    public int Id
    {
      get { return _DteCommand.ID; }
    }

    public bool IsAvailable
    {
      get { return _DteCommand.IsAvailable; }
    }

    public string Name
    {
      get { return _DteCommand.Name; }
    }

    public string LocalizedName
    {
      get { return _DteCommand.LocalizedName; }
    }
  }
}