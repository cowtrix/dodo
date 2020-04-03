import React from "react"
import { Link } from "react-router-dom"

import { fetchRebellion } from "app/domain/services/rebellion"
import { useFetch } from "app/domain/services/useFetch"
import { Button } from "app/components/button"

import styles from "./site-info.module.scss"

export const RebellionInfo = ({ rebellionId }) => {
	const rebellion = useFetch(fetchRebellion, rebellionId)

	if (!rebellion) {
		return <div>Loading</div>
	}

	return (
		<div className={styles.rebellionInfo}>
			<h3>{rebellion.Name}</h3>
			<div className={styles.rebellionDescription}>
				{rebellion.PublicDescription}
			</div>
			<Button
				as={<Link to={`/rebellion/${rebellionId}`} />}
				variant="outline"
				style={{ display: "inline-block" }}
			>
				JOIN REBELLION
			</Button>
		</div>
	)
}
