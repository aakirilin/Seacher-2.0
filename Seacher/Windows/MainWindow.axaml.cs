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
using System.Threading;

namespace Seacher
{
    public partial class MainWindow : Window
    {
        private readonly Settings settings;

        public MainWindow()
        {
            InitializeComponent();
            this.settings = Settings.Load();

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
                settings.Save();
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
                    SelectTable.Items.Add(new ComboBoxItemTable() { 
                        Content = title, 
                        DBTable = table, 
                        DBName = db.Name
                    });
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
                var db = settings[selectedItem.DBName];
                var qerryCreator = new QerryCreator(db, Conditions);
                var qerry = qerryCreator.Create(selectedItem.DBTable.Name);
                Debug.WriteLine(qerry);
                var type = CreateType(qerryCreator, selectedItem.DBTable.Name);
                var results = db
                    .SelectQerry(type, qerry)
                    .ToArray();

                ResultsDataGrid.ItemsSource = results;
            }
        }

        private Type CreateType(IEnumerable<DBColumnSettings> fields)
        {
            AppDomain myDomain = Thread.GetDomain();
            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = "MyDynamicAssembly";

            // To generate a persistable assembly, specify AssemblyBuilderAccess.RunAndSave.
            //AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndCollect);
            AssemblyBuilder myAsmBuilder = AssemblyBuilder.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndCollect);
            // Generate a persistable single-module assembly.
            ModuleBuilder myModBuilder =
                myAsmBuilder.DefineDynamicModule(myAsmName.Name);

            TypeBuilder myTypeBuilder = myModBuilder.DefineType("TempData",
                                                            TypeAttributes.Public);


            for (var i = 0; i < fields.Count(); i++)
            {
                var field = fields.ElementAt(i);
                if (!field.ShowInData)
                {
                    continue;
                }
                var fieldName = $"f{i}_{field.Name}";


                FieldBuilder customerNameBldr = myTypeBuilder.DefineField(fieldName + "_field",
                                                                typeof(string),
                                                                FieldAttributes.Private);

                PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(fieldName,
                                                             PropertyAttributes.HasDefault,
                                                             typeof(string),
                                                             null);

                // The property set and property get methods require a special
                // set of attributes.
                MethodAttributes getSetAttr =
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

                // Define the "get" accessor method for CustomerName.
                MethodBuilder custNameGetPropMthdBldr =
                    myTypeBuilder.DefineMethod("get_" + fieldName,
                                               getSetAttr,
                                               typeof(string),
                                               Type.EmptyTypes);

                ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

                custNameGetIL.Emit(OpCodes.Ldarg_0);
                custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
                custNameGetIL.Emit(OpCodes.Ret);

                // Define the "set" accessor method for CustomerName.
                MethodBuilder custNameSetPropMthdBldr =
                    myTypeBuilder.DefineMethod("set_" + fieldName,
                                               getSetAttr,
                                               null,
                                               new Type[] { typeof(string) });

                ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

                custNameSetIL.Emit(OpCodes.Ldarg_0);
                custNameSetIL.Emit(OpCodes.Ldarg_1);
                custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
                custNameSetIL.Emit(OpCodes.Ret);

                custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
                custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
            }
            Type? t = myTypeBuilder.CreateType();
            return t;
        }

        private Type CreateType(QerryCreator qerryCreator, string mainTable)
        {
            AppDomain myDomain = Thread.GetDomain();
            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = "MyDynamicAssembly";

            // To generate a persistable assembly, specify AssemblyBuilderAccess.RunAndSave.
            //AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndCollect);
            AssemblyBuilder myAsmBuilder = AssemblyBuilder.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndCollect);
            // Generate a persistable single-module assembly.
            ModuleBuilder myModBuilder =
                myAsmBuilder.DefineDynamicModule(myAsmName.Name);

            TypeBuilder myTypeBuilder = myModBuilder.DefineType("TempData",
                                                            TypeAttributes.Public);

            var fields = qerryCreator.GetAllColumns(mainTable);



            for (var i = 0; i < fields.Count(); i++)
            {
                var field = fields.ElementAt(i);
                if (!field.Column.ShowInData)
                {
                    continue;
                }
                var fieldName = $"{field.TableName}_{field.Column.Name}_t{field.TableIndex}_c{field.ColumnIndex}";


                FieldBuilder customerNameBldr = myTypeBuilder.DefineField(fieldName + "_field",
                                                                typeof(string),
                                                                FieldAttributes.Private);

                PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(fieldName,
                                                             PropertyAttributes.HasDefault,
                                                             typeof(string),
                                                             null);

                // The property set and property get methods require a special
                // set of attributes.
                MethodAttributes getSetAttr =
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

                // Define the "get" accessor method for CustomerName.
                MethodBuilder custNameGetPropMthdBldr =
                    myTypeBuilder.DefineMethod("get_" + fieldName,
                                               getSetAttr,
                                               typeof(string),
                                               Type.EmptyTypes);

                ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

                custNameGetIL.Emit(OpCodes.Ldarg_0);
                custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
                custNameGetIL.Emit(OpCodes.Ret);

                // Define the "set" accessor method for CustomerName.
                MethodBuilder custNameSetPropMthdBldr =
                    myTypeBuilder.DefineMethod("set_" + fieldName,
                                               getSetAttr,
                                               null,
                                               new Type[] { typeof(string) });

                ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

                custNameSetIL.Emit(OpCodes.Ldarg_0);
                custNameSetIL.Emit(OpCodes.Ldarg_1);
                custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
                custNameSetIL.Emit(OpCodes.Ret);

                custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
                custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
            }
            Type? t = myTypeBuilder.CreateType();
            return t;
        }
    }
}