module HoytTech.Concurrency.ConcurrentRingBuffer

open System.Threading
open HoytTech.Core;
    
module Cell =
    let EMPTY = 0
    let PENDING_SET = 1
    let VALUE = 2
    let PENDING_CLEAR = 3
    
    type t<'a> = {
        state: int32 ref
        value: 'a option ref
        version: int64 ref
    }
    
    let make =
        {
            state = ref EMPTY
            value = ref None
            version = ref 0L
        }
    
    let set t value =
        if Interlocked.CompareExchange(t.state, PENDING_SET, EMPTY) = EMPTY then
            Volatile.Write(t.value, Some(value))
            Interlocked.Increment(t.version) |> ignore
            Volatile.Write(t.state, VALUE)
            true
        else
            false
    
    let clear t =
        if Interlocked.CompareExchange(t.state, PENDING_CLEAR, VALUE) = VALUE then
            Volatile.Write(t.value, None)
            Interlocked.Increment(t.version) |> ignore
            Volatile.Write(t.state, EMPTY)
            true
        else
            false
     
    let get t =
        Volatile.Read(t.value)
            
    let hasValue t =
        Volatile.Read(t.state) = VALUE
    
    let empty t =
        Volatile.Read(t.state) = EMPTY
    
type t<'a> = {
    mask: int64
    buffer: Cell.t<'a> array
    length: int64
}

let make pos =
    let length =
        if PowerOf2.isPowerOfTwo pos then
            pos
        else
            PowerOf2.round pos
    let buffer = [| for i in 0 .. length -> Cell.make |]
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
    Cell.empty cell
    
let get t pos =
    let pos = calculatePos t pos
    let cell = t.buffer.[pos]
    Cell.get cell
    
let hasValue t pos =
    let pos = calculatePos t pos
    let cell = t.buffer.[pos]
    Cell.hasValue cell
    
let empty t pos =
    let pos = calculatePos t pos
    let cell = t.buffer.[pos]
    Cell.empty cell

let length t =
    t.length