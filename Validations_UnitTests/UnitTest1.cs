using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCR_BusinessLayer.Classes;
using OCR_BusinessLayer.Service;

namespace Validations_UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Validation_VariabilSymbol()
        {
            Evidence e = new Evidence();
            e.VariabilSymbol = " /1526g456";
            ValidationService.Validate(ref e);

            Assert.AreEqual("1526456", e.VariabilSymbol);

        }

        [TestMethod]
        public void Validation_KonstSymbol()
        {
            Evidence e = new Evidence();
            e.KonstSymbol = " /1526g456";
            ValidationService.Validate(ref e);

            Assert.AreEqual("1526", e.KonstSymbol);

        }

        [TestMethod]
        public void Validation_EvidenceNumber()
        {
            Evidence e = new Evidence();
            e.EvidenceNumber = " /1526g456";
            ValidationService.Validate(ref e);

            Assert.AreEqual("1526g456", e.EvidenceNumber);

        }
        [TestMethod]
        public void Validation_EvidenceNumber2()
        {
            Evidence e = new Evidence();
            e.EvidenceNumber = "=FV-1526456";
            ValidationService.Validate(ref e);

            Assert.AreEqual("FV-1526456", e.EvidenceNumber);

        }
        [TestMethod]
        public void Validation_EvidenceNumber3()
        {
            Evidence e = new Evidence();
            e.EvidenceNumber = "=FV 1526456";
            ValidationService.Validate(ref e);

            Assert.AreEqual("FV 1526456", e.EvidenceNumber);

        }

        [TestMethod]
        public void Validation_DocumentCreateDate()
        {
            Evidence e = new Evidence();
            e.DocumentCreateDate = " '/12062018";
            ValidationService.Validate(ref e);

            Assert.AreEqual("12.06. 2018", e.DocumentCreateDate);

        }
        [TestMethod]
        public void Validation_DocumentCreateDate2()
        {
            Evidence e = new Evidence();
            e.DocumentCreateDate = " '/12.06.2018";
            ValidationService.Validate(ref e);

            Assert.AreEqual("12.06. 2018", e.DocumentCreateDate);

        }
        [TestMethod]
        public void Validation_DocumentCreateDate3()
        {
            Evidence e = new Evidence();
            e.DocumentCreateDate = " '/12.06. 2018";
            ValidationService.Validate(ref e);

            Assert.AreEqual("12.06. 2018", e.DocumentCreateDate);

        }
        [TestMethod]
        public void Validation_DocumentCreateDate4()
        {
            Evidence e = new Evidence();
            e.DocumentCreateDate = " '/120618";
            ValidationService.Validate(ref e);

            Assert.AreEqual("12.06. 2018", e.DocumentCreateDate);

        }
        [TestMethod]
        public void Validation_DocumentCreateDate5()
        {
            Evidence e = new Evidence();
            e.DocumentCreateDate = " '/12 06 18";
            ValidationService.Validate(ref e);

            Assert.AreEqual("12.06. 2018", e.DocumentCreateDate);

        }
    }
}

