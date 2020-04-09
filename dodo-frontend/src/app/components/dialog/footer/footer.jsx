import React from "react"
import PropTypes from "prop-types"
import { Button } from "app/components"

import styles from "./footer.module.scss"

export const Footer = ({ update, buttonText = "Ok" }) => (
	<div className={styles.dialogFooter}>
		<Button
			variant="primary"
			onClick={update}
			type="button"
			className={styles.button}
		>
			{buttonText}
		</Button>
	</div>
)

Footer.propTypes = {
	update: PropTypes.func,
	buttonText: PropTypes.string
}
