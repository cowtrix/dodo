import React from "react";
import PropTypes from "prop-types";

import styles from "./button.module.scss";

export const Button = ({
	children,
	className,
	variant = "primary",
	as = <button />,
	disabled = false,
	...props
}) => {
	const buttonProps = {
		...props,
		className: [
			className,
			styles[variant],
			styles.button,
			disabled ? styles.disabled : "",
		].join(" "),
		disabled,
	};

	return React.cloneElement(as, buttonProps, children);
};

Button.propTypes = {
	children: PropTypes.node.isRequired,
	className: PropTypes.string,
	to: PropTypes.string,
	variant: PropTypes.string,
	onClick: PropTypes.func,
	as: PropTypes.node,
	disabled: PropTypes.bool
};
