import React from "react"
import PropTypes from "prop-types"
import styles from "./container.module.scss"

export const Container = ({ content, map = true }) => (
    <div className={map ? styles.wrapper : styles.wrapperWithoutMap}>
        <div className={styles.eventListContainer}>{content}</div>
    </div>
)

Container.propTypes = {
    content: PropTypes.node,
    map: PropTypes.bool
}
