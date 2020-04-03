import { useState, useEffect } from "react"

export const useFetch = (fetchFn, id) => {
	const [resource, setResource] = useState()

	useEffect(() => {
		const load = async () => {
			setResource(await fetchFn(id))
		}
		if (id) {
			load()
		}
	}, [fetchFn, id])

	return resource
}
