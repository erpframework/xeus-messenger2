using System.ComponentModel;
using System.Windows;
using agsXMPP.protocol.extensions.commands;
using xeus2.Properties;
using xeus2.xeus.Core;
using xeus2.xeus.UI;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Middle
{
    internal class CommandExecutor : WindowManager<string, CommandExecute>
    {
        private static readonly CommandExecutor _instance = new CommandExecutor();

        public static CommandExecutor Instance
        {
            get
            {
                return _instance;
            }
        }

        protected void DisplayQuestionaireInternal(Command command, Service service)
        {
            ServiceCommandExecution serviceCommandExecution = new ServiceCommandExecution(command, service);

            CommandExecute commandExecuteWindow;

            try
            {
                commandExecuteWindow = new CommandExecute(serviceCommandExecution);
                commandExecuteWindow.Show();
            }

            catch (WindowExistsException e)
            {
                commandExecuteWindow = (CommandExecute)e.ExistingWindow;
                commandExecuteWindow.Redisplay(serviceCommandExecution);
            }

            if (command.Status == Status.canceled)
            {
                EventInfo eventinfo = new EventInfo(string.Format(Resources.Event_CommandCancelled, service.Name));
                Events.Instance.OnEvent(this, eventinfo);

                commandExecuteWindow.Close();
            }
        }

        public void DisplayQuestionaire(Command command, Service service)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(DisplayQuestionaireInternal), command, service);
        }

        public Service ChooseCommand(Service service)
        {
            if (service.Commands.Count == 0)
            {
                return service;
            }

            if (service.Commands.Count == 1)
            {
                return service.Commands[0];
            }

            foreach (Service command in service.Commands)
            {
                if (JidUtil.CompareDiscoItem(command.DiscoItem, service.DiscoItem))
                {
                    return command;
                }
            }

            ChooseCommand chooseCommand = new ChooseCommand(service);
            chooseCommand.ShowDialog();

            if ((bool) chooseCommand.DialogResult)
            {
                return chooseCommand.Service;
            }
            else
            {
                return null;
            }
        }

        #region Nested type: DisplayCallback

        private delegate void DisplayCallback(Command command, Service service);

        #endregion
    }
}