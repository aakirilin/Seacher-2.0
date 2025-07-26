using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Seacher.Commons;
using Seacher.Controlls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Seacher
{
    public partial class MainWindow : Window
    {
        private readonly Settings settings;

        public MainWindow(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;

            SelectTable.SelectionChanged += (s, e) => {
                var index = SelectTable.SelectedIndex;

            };


            UpdateSelectTableComboBox();
        }

        private void Settings_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var window = new SettingsWindow(settings);
            window.Show();
            window.Closed += (s, e) =>
            {
                UpdateSelectTableComboBox();
            };
        }

        private void UpdateSelectTableComboBox()
        {
            SelectTable.Items.Clear();
            foreach (var db in settings.DBSettings)
            {
                foreach (var table in db.DBTables)
                {
                    var title = $"{db.Name}->{table.Name}";
                    SelectTable.Items.Add(new ComboBoxItemTable() { Content = title, DBTable = table, DBName = db.Name });
                }
            }

            CreateConditions();
        }

        private void CreateConditions()
        {
            Conditions.Children.Clear();
            var selectedItem = SelectTable.SelectedItem as ComboBoxItemTable;
            if (selectedItem != null)
            {
                Conditions.Children.AddRange(selectedItem.DBTable.CreateConditions());
            }
        }

        private void SelectTable_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            CreateConditions();
        }

        private void FindButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var selectedItem = SelectTable.SelectedItem as ComboBoxItemTable;
            if (selectedItem != null)
            {
                var qerry = selectedItem.DBTable.CreateQerry(Conditions);
                var db = settings[selectedItem.DBName];

                var results = db.SelectQerry(qerry).ToArray();

                var type = CreateType(selectedItem.DBTable.Columns);

                
            }
        }

        private Type CreateType(IEnumerable<DBColumnSettings> fields)
        {
            var aName = new AssemblyName("DynamicAssembly");

            var ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndCollect);

            var mb = ab.DefineDynamicModule(ab.GetName().FullName);

            var tb = mb.DefineType("DynamicType", TypeAttributes.Public);

            foreach (var field in fields)
            {
                PropertyBuilder property = tb.DefineProperty(
                    field.Name,
                    PropertyAttributes.HasDefault,
                    typeof(string),
                    null);

                MethodAttributes getSetAttr =
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

                MethodBuilder get = tb.DefineMethod(
                    $"get_{field.Name}",
                    getSetAttr,
                    typeof(string),
                    Type.EmptyTypes);

                ILGenerator pGetIL = get.GetILGenerator();
                pGetIL.Emit(OpCodes.Ldarg_0);
                pGetIL.Emit(OpCodes.Ldfld);
                pGetIL.Emit(OpCodes.Ret);

                MethodBuilder set = tb.DefineMethod(
                    $"set_{field.Name}",
                    getSetAttr,
                    null,
                    new Type[] { typeof(string) });

                ILGenerator numberSetIL = set.GetILGenerator();
                numberSetIL.Emit(OpCodes.Ldarg_0);
                numberSetIL.Emit(OpCodes.Ldarg_1);
                numberSetIL.Emit(OpCodes.Stfld);
                numberSetIL.Emit(OpCodes.Ret);

                property.SetGetMethod(get);
                property.SetSetMethod(set);

                var attributeCtor = tb.GetConstructor([typeof(DisplayNameAttribute)]);
                CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(attributeCtor, [field.Name]);
                property.SetCustomAttribute(caBuilder);
            }

            Type? t = tb.CreateType();
            return t;
        }
    }
}