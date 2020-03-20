import React from "react";
import PropTypes from "prop-types";
import { Link } from "react-router-dom";

import styles from "./button.module.scss";

const Button = ({ children, variant = "primary", to, onClick }) => {
	const className = `${styles.button} ${styles[variant]}`;

	return onClick ? (
		<button className={className} onClick={onClick}>
			{children}
		</button>
	) : (
		<Link to={to} className={className}>
			{children}
		</Link>
	);
};

Button.propTypes = {
	children: PropTypes.node.isRequired,
	to: PropTypes.string,
	variant: PropTypes.string,
	onClick: PropTypes.func
};

export default Button;
