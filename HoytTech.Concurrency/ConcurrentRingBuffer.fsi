module HoytTech.Concurrency.ConcurrentRingBuffer

type t<'a>

val make : int32 -> t<'a>

/// Set the value in the record buffer.
/// <param name="buffer">The buffer to write to.</param>
/// <param name="pos">The position to write in the buffer.</param>
/// <param name="value">The value to write.</param>
/// <returns>true if the value is set.</returns>
val set: t<'a> -> int64 -> 'a -> bool

/// Clears a value out of the buffer.
/// <param name="buffer">The buffer to clear the value from.</param>
/// <param name="pos">The position to clear.</param>
/// <returns>true if the position was cleared.</returns>
val clear: t<'a> -> int64 -> bool

/// Gets the value at the position.
/// <param name="buffer">THe buffer to get the value out of.</param>
/// <param name="pos">The position to add.</param>
/// <returns>The value at that position.</returns>
val get: t<'a> -> int64 -> Option<'a>

/// Check to see if there is a value in the buffer.
/// <param name="buffer">The buffer to check and see if has a value.</param>
/// <param name="pos">The position to check.</param>
/// <returns>true if there is a value at that position.</returns>
val hasValue: t<'a> -> int64 -> bool

/// Checks to see if a value is empty.
/// <param name="buffer">The buffer to check and see if it has a value.</param>
/// <param name="pos">The position to check.</param>
/// <returns>True if the value is empty.</returns>
val empty: t<'a> -> int64 -> bool

/// Gets the length of the buffer.
/// <param name="buffer">The buffer to get the size for.</param>
/// <returns>The size of the value.</returns>
val length: t<'a> -> int64

