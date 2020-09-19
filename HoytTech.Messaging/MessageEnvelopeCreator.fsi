module HoytTech.Messaging.MessageEnvelopeCreator

type t

/// Used to make a new envelope creator.
val make : string -> int -> t;

type packet = {
    number: int
    total: int
    body: string
}

val processEnvelopes : t -> (packet -> unit) -> unit

val getEnvelope: t -> int -> option<packet>

