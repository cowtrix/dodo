import React from "react"
import { Button } from "app/components/button"
import { Events } from "./events"

import styles from "./filter.module.scss"

export const Filter = () => (
	<div className={styles.filter}>
		<Events />
	</div>
)

Filter.propTypes = {}
