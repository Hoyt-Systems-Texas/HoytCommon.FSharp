module HoytTech.Concurrency.ConcurrentRingBuffer

open System.Threading
open HoytTech.Core;
    
module Cell =
    
    type t<'a> = {
        value: 'a option ref
        version: int64 ref
    }
    
    let make<'a> =
        let value: Option<'a> = None
        {
            value = ref value
            version = ref 0L
        }
    
    let set t value =
        Option.isNone(Interlocked.CompareExchange(t.value, Some(value), None))
    
    let clear t =
        Volatile.Write(t.value, None);
        true
     
    let get t =
        Volatile.Read(t.value)
            
    let hasValue t =
        Option.isSome(Volatile.Read(t.value))
    
    let empty t =
        Option.isNone(Volatile.Read(t.value))
    
type t<'a> = {
    mask: int64
    buffer: Cell.t<'a> array
    length: int64
}

let make<'a> pos =
    let length =
        if PowerOf2.isPowerOfTwo pos then
            pos
        else
            PowerOf2.round pos
    let buffer = [| for i in 1 .. length -> Cell.make<'a> |]
    let mask = length - 1
    {
        mask = int64 mask
        buffer = buffer
        length = int64 length
    }
        

let calculatePos t pos =
    int32 <| (pos &&& t.mask)
    
let set t pos value =
    let pos =  calculatePos t pos
    let cell = t.buffer.[pos]
    Cell.set cell value
    
let clear t pos =
    let pos = calculatePos t pos
    let cell = t.buffer.[pos]
    Cell.clear cell
    
let get t pos =
    let pos = calculatePos t pos
    let cell = t.buffer.[pos]
    Cell.get cell
    
let hasValue t pos =
    let idx = calculatePos t pos
    let cell = t.buffer.[idx]
    Cell.hasValue cell
    
let empty t pos =
    let pos = calculatePos t pos
    let cell = t.buffer.[pos]
    Cell.empty cell

let length t =
    t.length