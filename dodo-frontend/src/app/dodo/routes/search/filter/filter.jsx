import React from "react"
import { Selector } from "app/components"

import { Events } from "./events"
import { Distance } from "./distance"
import { Date } from "./date"

import styles from "./filter.module.scss"

const title = "Filter..."

export const Filter = () => (
	<Selector
		title={title}
		content={
			<div className={styles.filter}>
				<Events />
				<Distance />
				<Date />
			</div>
		}
	/>
)

Filter.propTypes = {}
