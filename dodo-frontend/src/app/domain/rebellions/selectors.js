import { path } from "ramda"

export const rebellions = state => path(["domain", "rebellions"], state)

export const currentRebellion = state =>
	path(["domain", "currentRebellion"], state)
