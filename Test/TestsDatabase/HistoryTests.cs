﻿using System;
using System.Collections.Generic;
using System.Text;
using Keas.Core.Domain;
using TestHelpers.Helpers;
using Xunit;

namespace Test.TestsDatabase
{
    [Trait("Category","DatabaseTests")]
    public class HistoryTests
    {
        #region Reflection of Database

        [Fact]
        public void TestDatabaseFieldAttributes()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("Access", "Keas.Core.Domain.Access", new List<string>()));
            expectedFields.Add(new NameAndType("AccessId", "System.Nullable`1[System.Int32]", new List<string>()));
            expectedFields.Add(new NameAndType("ActedDate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("ActionType", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Actor", "Keas.Core.Domain.User", new List<string>()));
            expectedFields.Add(new NameAndType("ActorId", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("AssetType", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Description", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
            }));
            expectedFields.Add(new NameAndType("Equipment", "Keas.Core.Domain.Equipment", new List<string>()));
            expectedFields.Add(new NameAndType("EquipmentId", "System.Nullable`1[System.Int32]", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Key", "Keas.Core.Domain.Key", new List<string>()));
            expectedFields.Add(new NameAndType("KeyId", "System.Nullable`1[System.Int32]", new List<string>()));
            expectedFields.Add(new NameAndType("Serial", "Keas.Core.Domain.Serial", new List<string>()));
            expectedFields.Add(new NameAndType("SerialId", "System.Nullable`1[System.Int32]", new List<string>()));
            expectedFields.Add(new NameAndType("Target", "Keas.Core.Domain.Person", new List<string>()));
            expectedFields.Add(new NameAndType("TargetId", "System.Nullable`1[System.Int32]", new List<string>()));
            expectedFields.Add(new NameAndType("Workstation", "Keas.Core.Domain.Workstation", new List<string>()));
            expectedFields.Add(new NameAndType("WorkstationId", "System.Nullable`1[System.Int32]", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(History));
        }

        #endregion Reflection of Database
    }
}
