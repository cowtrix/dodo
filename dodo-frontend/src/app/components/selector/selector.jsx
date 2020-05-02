import React, { Fragment, useState, cloneElement } from "react"
import { Button, Dialog } from "app/components"

import styles from "./selector.module.scss"

export const Selector = ({ title, content }) => {
	const [dialogOpen, setDialogOpen] = useState(false)

	return (
		<Fragment>
			<div className={styles.mobile}>
				<Button
					variant="primary"
					onClick={() => setDialogOpen(true)}
					className={styles.button}
				>
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
		</Fragment>
	)
}
