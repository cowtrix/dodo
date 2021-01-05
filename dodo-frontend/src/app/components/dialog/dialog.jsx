import React from "react"
import PropTypes from "prop-types"
import MuDialog from "@material-ui/core/Dialog"
import { Header } from "./header"
import { Footer } from "./footer"
import { Content } from "./content"

import styles from "./dialog.module.scss"

export const Dialog = ({
	active,
	title,
	content,
	update,
	buttonText,
	close,
	fullScreen
}) => (
	<MuDialog open={active} className={styles.dialogWrapper} fullWidth={fullScreen} fullScreen={fullScreen} PaperProps={{square: true}}>
		<div className={styles.dialog}>
			<Header onClose={close} title={title} />
			<Content content={content} />
			{update && <Footer update={update} buttonText={buttonText} />}
		</div>
	</MuDialog>
)

Dialog.defaultProps = {
	fullScreen: true
}

Dialog.propTypes = {
	active: PropTypes.bool,
	title: PropTypes.string,
	content: PropTypes.node,
	update: PropTypes.func,
	buttonText: PropTypes.string,
	fullScreen: PropTypes.bool
}
