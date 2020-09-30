import React, { useState } from "react"
import { Button, Dialog, Icon } from "app/components"

import styles from "./selector.module.scss"

export const Selector = ({ title, content }) => {
	const [dialogOpen, setDialogOpen] = useState(false)

	return (
		<>
			<div className={styles.mobile}>
				<Button
					variant="link"
					onClick={() => setDialogOpen(true)}
					className={styles.button}
				>
					<Icon icon="sliders-h"/>
					{title}
				</Button>
				<Dialog
					active={dialogOpen}
					title={title}
					content={content}
					close={() => setDialogOpen(false)}
					update={() => setDialogOpen(false)}
				/>
			</div>
			<div className={styles.desktop}>{content}</div>
		</>
	)
}
