module HoytTech.Core.Validation

open System.Text.RegularExpressions

module StringValidation =
    type t = {
        FieldName: string
        Required: bool
        MinLength: int32
        MaxLength: int32    
    }

    let validate t (value: string) =
        let value = value.Trim()
        let length = String.length value
        if t.Required && length = 0 then
            [t.FieldName + " is required."]
        else [
            if t.MinLength > length then
                yield sprintf "%s must be at least %i" t.FieldName t.MinLength
            if t.MaxLength < length then
                yield sprintf "%s must be less than %i" t.FieldName t.MaxLength
        ]
    
    let validateOption validation (value: Option<string>) =
        match value with
        | Some(value) -> validate validation value
        | None -> []
        
module EmailValidation =
    type t = StringValidation.t
    let regex = Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
    let validate t (value: string) =
        let value = value.Trim()
        StringValidation.validate t value @ [
            if String.length value > 0 && not <| regex.IsMatch(value) then
                yield sprintf "%s is not a valid email address." t.FieldName
        ]
        
module NumberValidation = 
    type t<'a> = {
        FieldName: string
        MinValue: 'a
        MaxValue: 'a
    }

    let validate t value =
        [
            if t.MinValue > value then
                yield sprintf "%s must be at least %i" t.FieldName t.MinValue
            if t.MaxValue < value then
                yield sprintf "%s must be less than %i" t.FieldName t.MaxValue
        ]
