import { path } from "ramda"

export const currentEvent = state => path(["domain", "currentEvent"], state)
