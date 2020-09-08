namespace HoytTech.Core

module Errors =
    type t = List<string>

module ResultMonad =
    type t<'a> =
        | Success of 'a
        | Busy
        | Error of Errors.t
        | AccessDenied

    let bind exp func =
        match exp with
        | Success a -> func(a)
        | Error e -> Error e
        | Busy -> Busy
        | AccessDenied -> AccessDenied
        
    type ResultMonadBuilder() =
        member x.Bind(comp, func) = bind comp func
        member x.Result(v) = Success v
        
module Clock =
    open HoytTech.CSharp
    
    let timeInFrequency =
        SystemCalls.GetTimestamp
        
    let millsToFrequency mills =
        SystemCalls.MillsToFrequency mills
  