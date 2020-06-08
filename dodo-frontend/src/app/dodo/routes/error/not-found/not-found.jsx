import React from "react"
import { ContentPage } from "app/components"
import { PageTitle } from "app/components"
import styles from "./not-found.module.scss"

export const NotFound = () => (
	<ContentPage>
		<div className={styles.notFoundContainer}>
			<PageTitle
				title="Page not found"
				subTitle="Sorry, we couldn't find what you're looking for."
			/>
		</div>
	</ContentPage>
)
