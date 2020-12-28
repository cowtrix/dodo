import React, { useReducer } from "react";
import PropTypes from "prop-types";

import { Icon } from "../icon";

import styles from "./expand-panel.module.scss";

// TODO: add expand / retract animation styles and logic
export const ExpandPanel = ({
	header,
	headerClassName,
	toggleState,
	children,
}) => {
	const [expanded, toggleExpanded] = useReducer((s) => !s, false);

	return (
		<>
			<div className={[styles.header, headerClassName].join(" ")}>
				<div>{header}</div>
				<Icon
					icon={`chevron-${expanded ? "up" : "down"}`}
					className={styles.icon}
					onClick={toggleExpanded}
				/>
			</div>
			{expanded && <div className={styles.content}>{children}</div>}
		</>
	);
};

ExpandPanel.propTypes = {
	header: PropTypes.node,
	headerClassName: PropTypes.string,
	toggleState: PropTypes.func,
	children: PropTypes.node,
};
