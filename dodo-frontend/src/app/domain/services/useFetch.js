import { useState, useEffect } from "react"

export const useFetch = (fetchFn, id) => {
	// Use pre-cached value if available
	const result = fetchFn(id)
	const [resource, setResource] = useState(
		result && typeof result.then !== "function" ? result : null
	)

	useEffect(() => {
		const load = async () => {
			setResource(await result)
		}
		if (id) {
			load()
		}
	}, [fetchFn, id])

	return resource
}
