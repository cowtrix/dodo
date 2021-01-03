import PropTypes from "prop-types";
import React from "react";
import { Button } from "../../button";
import styles from "./submit.module.scss";

export const Submit = ({ submit = () => {}, value, className, disabled }) => (
	<div className={styles.submit}>
		<Button
			variant="cta"
			onClick={() => submit()}
			{...{ className, disabled }}
		>
			<div>{value}</div>
		</Button>
	</div>
);

Submit.propTypes = {
	submit: PropTypes.func,
	value: PropTypes.string,
	className: PropTypes.string,
	disabled: PropTypes.bool,
};
