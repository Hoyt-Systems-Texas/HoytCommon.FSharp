module HoytTech.Concurrency.Queue

module Mpmc =
    
    type t<'a>
    
    val make<'a> : int32 -> t<'a>
    
    val procedureIndex: t<'a> -> int64
    
    val consumerIndex: t<'a> -> int64
    
    val offer: t<'a> -> 'a -> bool
    
    val poll: t<'a> -> Option<'a>
    
module SkipQueue =
    
    type t<'a>
    
    val make<'a> : int32 -> int32 -> t<'a>
    
    val offer: t<'a> -> 'a -> bool
    
    val poll: t<'a> -> Option<'a>
    
    val defer: t<'a> -> 'a -> bool
    
    val reset: t<'a> -> unit