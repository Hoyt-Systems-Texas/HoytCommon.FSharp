module HoytTech.Concurrency.Queue

module Mpsc =
    
    type t<'a>
    
    val procedureIndex: t<'a> -> int64
    
    val consumerIndex: t<'a> -> int64
    
    val offer: t<'a> -> 'a -> bool
    
    val poll: t<'a> -> Option<'a>