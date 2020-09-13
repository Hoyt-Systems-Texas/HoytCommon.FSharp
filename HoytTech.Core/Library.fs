namespace HoytTech.Core

open System
open System.Security.Cryptography

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
        
module PowerOf2 =
    
    let round value =
        int32 <| HoytTech.CSharp.PowerOfTwo.RoundToPowerOfTwo value
        
    let isPowerOfTwo value =
        HoytTech.CSharp.PowerOfTwo.IsPowerOfTwo value
 
 module Security =
     
     let randomNumberGenerator = RNGCryptoServiceProvider()
     
     /// Generates a GUID where all of the digits are from a secure number generator.
     let secureGuid () =
         let bytes = Array.zeroCreate 32
         randomNumberGenerator.GetBytes(bytes)
         Guid(bytes)
         