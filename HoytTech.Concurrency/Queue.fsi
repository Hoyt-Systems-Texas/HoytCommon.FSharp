module HoytTech.Concurrency.Queue

module Mpmc =
    
    type t<'a>
    
    val make<'a> : int32 -> t<'a>
    
    val procedureIndex: t<'a> -> int64
    
    val consumerIndex: t<'a> -> int64
    
    val offer: t<'a> -> 'a -> bool
    
    val poll: t<'a> -> Option<'a>
    