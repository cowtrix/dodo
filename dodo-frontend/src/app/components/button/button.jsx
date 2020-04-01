import React, { Fragment } from "react";
import PropTypes from "prop-types";
import { Link } from "react-router-dom";

import styles from "./button.module.scss";

export const Button = ({
	children,
	className,
	style = {},
	variant = "primary",
	to,
	onClick
}) => {
	const buttonStyle = `${styles.button} ${styles[variant]} `;

	return (
		<Fragment>
			{onClick ? (
				<button className={buttonStyle} onClick={onClick} style={style}>
					{children}
				</button>
			) : (
				<Link to={to} className={`${className} ${styles.link}`} style={style}>
					{children}
				</Link>
			)}
		</Fragment>
	);
};

Button.propTypes = {
	children: PropTypes.node.isRequired,
	to: PropTypes.string,
	variant: PropTypes.string,
	onClick: PropTypes.func
};
