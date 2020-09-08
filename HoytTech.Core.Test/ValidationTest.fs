module HoytTech.Core.Test.ValidationTest

open HoytTech.Core
open NUnit.Framework
open Validation.StringValidation
open Validation.NumberValidation
open Validation

[<TestFixture>]
type ValidationTestClass () =
    
    let defaultStringValidation = {
        FieldName = "Test"
        Required = true
        MinLength = 2
        MaxLength = 8
    }
    
    let defaultIntValidation = {
        FieldName = "Test"
        MinValue = 5
        MaxValue = 10
    }

    [<Test>]
    member this.ValidateRequiredFailureTest() =
        let result = StringValidation.validate defaultStringValidation ""
        match result with
        | [] -> Assert.Fail("Expected a failure.")
        | [ required ] -> Assert.IsTrue(required.Contains("required"))
        | _ -> Assert.Fail("More than one result returned")
        
    [<Test>]
    member this.ValidateRequiredTest() =
        let result = StringValidation.validate defaultStringValidation "123"
        Assert.AreEqual([], result);
        
    [<Test>]
    member this.ValidateMinLengthFailureTest() =
        let result = StringValidation.validate defaultStringValidation "H"
        match result with
        | [] -> Assert.Fail("Expected a failure.")
        | [ minLength ] -> Assert.IsTrue(minLength.Contains("least"))
        | _ -> Assert.Fail("Multiple results returned.")
    
    [<Test>]
    member this.ValidateMaxLengthFailureTest() =
        let result = StringValidation.validate defaultStringValidation "123456789"
        match result with
        | [] -> Assert.Fail("Expected a failure.")
        | [ maxLength ] -> Assert.IsTrue(maxLength.Contains("less"))
        | _ -> Assert.Fail("Multiple results returned.")
        
    [<Test>]
    member this.ValidateMaxLengthTest() =
        let result = StringValidation.validate defaultStringValidation "12345678"
        Assert.AreEqual([], result)
        
    [<Test>]
    member this.ValidateEmailFailureTest() =
        let result = EmailValidation.validate defaultStringValidation "mee"
        match result with
        | [] -> Assert.Fail "Expected a failure"
        | [ email ] -> Assert.IsTrue(email.Contains("email"))
        | _ -> Assert.Fail("More than one error")
        
    [<Test>]
    member this.ValidateEmailTest() =
        let result = EmailValidation.validate defaultStringValidation "me@me.co"
        Assert.AreEqual(result, [])
        
    [<Test>]
    member this.ValidationNumberTest() =
        let result = NumberValidation.validate defaultIntValidation 7
        Assert.AreEqual([], result)
        
    [<Test>]
    member this.ValidationNumberMin() =
        let result = NumberValidation.validate defaultIntValidation 4
        match result with
        | [] -> Assert.Fail("Expected an error.")
        | [result] -> Assert.IsTrue(result.Contains("least"))
        | _ -> Assert.Fail("Multiple errors.")
        
    member this.ValidateNumberMax() =
        let result = NumberValidation.validate defaultIntValidation 11
        match result with
        | [] -> Assert.Fail("Expected an error.")
        | [result] -> Assert.IsTrue(result.Contains("less"))
        | _ -> Assert.Fail("Multiple errors.")
