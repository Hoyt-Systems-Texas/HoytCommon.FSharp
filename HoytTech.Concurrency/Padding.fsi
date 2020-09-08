module HoytTech.Concurrency.Padding

module PaddedLong =
    
    type t
    
    val make : int64 -> t
    
    val write : t -> int64 -> unit
    
    val read : t -> int64
    
    val compareExchange : t -> int64 -> int64 -> bool