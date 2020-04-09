import React from "react";
import PropTypes from "prop-types";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import styles from "./header.module.scss";

export const Header = ({ title, onClose }) => (
	<div className={styles.dialogHeader}>
		<h3>{title}</h3>
		{onClose ? (
			<IconButton aria-label="close" onClick={onClose}>
				<CloseIcon />
			</IconButton>
		) : null}
	</div>
);

Header.propTypes = {
	title: PropTypes.string,
	onClose: PropTypes.func
};
