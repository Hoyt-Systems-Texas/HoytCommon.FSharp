module HoytTech.Concurrency.Padding

open System.Threading

module PaddedLong =
    
    type t = {
        value: int64 ref
        value1: int64
        value2: int64
        value3: int64
        value4: int64
        value5: int64
        value6: int64
        value7: int64
        value8: int64
        value9: int64
        value10: int64
        value11: int64
        value12: int64
        value13: int64
        value14: int64
        value15: int64
    }
    
    let make value =
        {
            value = ref value
            value1 = 0L
            value2 = 0L
            value3 = 0L
            value4 = 0L
            value5 = 0L
            value6 = 0L
            value7 = 0L
            value8 = 0L
            value9 = 0L
            value10 = 0L
            value11 = 0L
            value12 = 0L
            value13 = 0L
            value14 = 0L
            value15 = 0L
        }
        
    let write t value =
        Volatile.Write(t.value, value)

    let read t =
        Volatile.Read(t.value)
        
    let compareExchange t value expected =
        Interlocked.CompareExchange(t.value, value, expected) = expected
        
